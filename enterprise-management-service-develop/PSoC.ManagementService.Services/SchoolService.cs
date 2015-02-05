using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Services.Extentions;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    public class SchoolService : ISchoolService
    {
        private IUnitOfWork UnitOfWork { get; set; }
        private readonly ISchoolRepository _schoolRepository;

        public SchoolService(IUnitOfWork unitOfWork, ISchoolRepository schoolRepository)
        {
            UnitOfWork = unitOfWork;
            _schoolRepository = schoolRepository;
        }

        /// <summary>
        /// Retrieve all items from the database
        /// </summary>
        /// <returns>List of database model objects</returns>
        public async Task<IList<School>> GetAsync()
        {
            try
            {
                var resultList = new List<School>();

                var dataRepository = UnitOfWork.GetDataRepository<SchoolDto, Guid>();
                foreach (var item in await dataRepository.GetAsync().ConfigureAwait(false))
                {
                    resultList.Add((School)item);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.SchoolServiceException(ex.Message, logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }

        public async Task<School> GetByIdAsync(Guid schoolId)
        {
            School school;

            try
            {
                SchoolDto schoolDto = await _schoolRepository.GetByIdAsync(schoolId).ConfigureAwait(false);
                school = (School)schoolDto;
            }
            catch (Exception e)
            {
                PEMSEventSource.Log.SchoolServiceException(e.ToString());
                school = null;
            }

            return school;
        }

        public async Task<School> CreateAsync(School school)
        {
            try
            {
                SchoolDto schoolDto = (SchoolDto)school;
                schoolDto = await _schoolRepository.InsertAsync(schoolDto).ConfigureAwait(false);
                school = (School) schoolDto;
            }
            catch (Exception e)
            {
                PEMSEventSource.Log.SchoolServiceException(e.ToString());
                school = null;
            }

            return school;
        }

        public Task<School> UpdateAsync(School entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve all items from the database with access check
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns>List of database model objects</returns>
        public async Task<IList<School>> GetAsync(String username)
        {
            AdminDto admin = null;
            try
            {
                var resultList = new List<School>();

                // Access check
                admin = await UnitOfWork.AdminRepository.GetByUsernameAsync(username).ConfigureAwait(false);

                if (admin != null)
                {
                    switch (admin.AdminType)
                    {
                        case AdminType.GlobalAdmin:
                            resultList.AddRange(await GetAsync().ConfigureAwait(false));
                            break;
                        case AdminType.DistrictAdmin:
                            Guid? key = null;
                            if (admin.District != null)
                                key = admin.District.DistrictId;

                            var schools = await GetByDistrictIdAsync(key).ConfigureAwait(false);

                            if (schools != null)
                                resultList.AddRange(schools);
                            break;
                        case AdminType.SchoolAdmin:
                            if (admin.School != null)
                            {
                                var schoolDto = await UnitOfWork.SchoolRepository.GetByIdAsync(admin.School.SchoolID).ConfigureAwait(false);
                                if (schoolDto != null)
                                    resultList.Add((School)schoolDto);
                            }
                            if (resultList.Count <= 0)
                            {
                                resultList.AddRange(await GetAsync().ConfigureAwait(false));
                            }
                            break;
                    }
                }
                else
                {
                    PEMSEventSource.Log.SchoolServiceException("username not found");
                }

                return resultList;
            }
            catch (Exception ex)
            {
                var userId = (admin != null) ? admin.User.UserID.ToString() : null;
                PEMSEventSource.Log.SchoolServiceException(ex.Message, userId: userId, logRequest: new LogRequest { Exception = ex });
                throw;
            }

        }

        /// <summary>
        /// Retrieve an item from the database
        /// </summary>
        /// <param name="key">Unique database item identifier, i.e. value of primary key</param>
        /// <returns>A database model object</returns>
        public async Task<IList<School>> GetByDistrictIdAsync(Guid? key)
        {
            try
            {
                var resultList = new List<School>();

                IList<SchoolDto> result; 
                if(key!=null)
                    result = await UnitOfWork.SchoolRepository.GetByDistrictIdAsync(key.Value).ConfigureAwait(false);     
                else
                    result = await UnitOfWork.SchoolRepository.GetAsync().ConfigureAwait(false);   

                if(result !=null)
                {
                    resultList.AddRange(result.ToSchoolList());
                }

                return resultList;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.SchoolServiceException(ex.Message, key.ToString(), logRequest: new LogRequest { Exception = ex });
                throw;
            }
        }
    }
}
