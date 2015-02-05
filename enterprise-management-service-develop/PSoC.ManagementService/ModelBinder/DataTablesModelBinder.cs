using System;
using System.Web.Mvc;

using PSoC.ManagementService.Models;

namespace PSoC.ManagementService.ModelBinder 
{
    public class DataTablesModelBinder : IModelBinder 
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            if (bindingContext.ModelType != typeof(DataTablePageRequestModel))
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, "Cannot convert value to DataTablePageViewModel");
                return null;
            }
         
            var pageRequest = new DataTablePageRequestModel
            {
                Echo = GetPageRequestParamValue<Int32>(bindingContext, "Echo", "sEcho"),
                DisplayStart = GetPageRequestParamValue<Int32>(bindingContext, "DisplayStart", "iDisplayStart"),
                DisplayLength = GetPageRequestParamValue<Int32>(bindingContext, "DisplayLength", "iDisplayLength"),
                ColumnNames = GetPageRequestParamValue<string>(bindingContext, "Columns", "sColumns"),
                Columns = GetPageRequestParamValue<Int32>(bindingContext, "Columns", "iColumns"),
                Search = GetPageRequestParamValue<string>(bindingContext, "Search", "sSearch"),
                SortingCols = GetPageRequestParamValue<Int32>(bindingContext, "SortingCols", "iSortingCols"),
            };
            return pageRequest;
        }

        private T GetPageRequestParamValue<T>(ModelBindingContext context, string propertyName, string altPropertyName = "")
        {
            ValueProviderResult valueResult = context.ValueProvider.GetValue(propertyName) ?? context.ValueProvider.GetValue(altPropertyName);
            if (valueResult == null)
                return default(T);
            return (T)valueResult.ConvertTo(typeof(T));
        }
    }
}