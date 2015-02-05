using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSoC.ManagementService.Models
{
    public class JsonErrorModel
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}