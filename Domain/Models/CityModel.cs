using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CityModel
    {
        public string CityNameRu { get; set; }
        public string RegionNameRu { get; set; }
        public string RegionNameEn { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
    }

}
