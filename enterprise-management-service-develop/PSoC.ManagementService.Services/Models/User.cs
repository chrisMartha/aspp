using System;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    public class User
    {
        public User()
        {
        }

        public User(UserDto user)
        {
            UserId = user.UserID;
            Username = user.Username;
            UserType = user.UserType;
            Created = user.Created;
        }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string UserType { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Cast User to UserDto
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static explicit operator UserDto(User user)
        {
            if (user == null) return null;
            return new UserDto()
            {
                UserID = user.UserId,
                Username = user.Username,
                UserType = user.UserType
            };
        }

        /// <summary>
        /// Cast UserDto to User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static explicit operator User(UserDto user)
        {
            if (user == null) return null;
            return new User()
            {
                UserId = user.UserID,
                Username = user.Username,
                UserType = user.UserType,
                Created = user.Created
            };
        }
    }
}
