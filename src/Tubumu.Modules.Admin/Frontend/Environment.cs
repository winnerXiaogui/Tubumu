using System;
using System.Collections.Generic;
using System.Text;

namespace Tubumu.Modules.Admin.Frontend
{
    public class Environment
    {
        public bool IsDevelopment { get; set; }
        public string ProductionHost { get; set; }
        public string DevelopmentHost { get; set; }
    }
}
