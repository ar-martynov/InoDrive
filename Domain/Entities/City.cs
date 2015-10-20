using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class City
    {
        private ICollection<Region> _regions { get; set; }
        public int CityId { get; set; }
        public string CityNameRu { get; set; }
        public string CityNameEn { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

        public virtual ICollection<Region> Regions
        {
            get { return _regions ?? (_regions = new List<Region>()); }
            protected set { _regions = value; }
        }
        
    }
}
