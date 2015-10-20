using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Region
    {
        private ICollection<City> _cities { get; set; }
        public int RegionId { get; set; }
        public string RegionNameEn { get; set; }
        public string RegionNameRu { get; set; }

        public virtual ICollection<City> Cities
        {
            get { return _cities ?? (_cities = new List<City>()); }
            protected set { _cities = value; }
        }
    }
}
