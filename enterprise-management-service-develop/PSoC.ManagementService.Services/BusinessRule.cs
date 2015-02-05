using System;

namespace PSoC.ManagementService.Services
{
    public class BusinessRule
    {
        private readonly string _ruleDescription;

        public BusinessRule(string ruleDescription)
        {
            _ruleDescription = ruleDescription;
        }

        public String RuleDescription
        {
            get
            {
                return _ruleDescription;
            }
        }
    }
}
