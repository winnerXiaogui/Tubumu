using System;
using System.Collections.Generic;

namespace Tubumu.Modules.Admin.Entities
{
    public partial class MobileValidationCode
    {
        public string Mobile { get; set; }
        public string ValidationCode { get; set; }
        public int TypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? FinishVerifyDate { get; set; }
        public int VerifyTimes { get; set; }
        public int MaxVerifyTimes { get; set; }
    }
}
