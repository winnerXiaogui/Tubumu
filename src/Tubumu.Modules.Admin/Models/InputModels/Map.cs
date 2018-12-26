using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Modules.Admin.Models.InputModels
{
    public class CoordinateInput
    {
        [DisplayName("经度")]
        public double Longitude { get; set; }
        [DisplayName("维度")]
        public double Latitude { get; set; }

    }
}
