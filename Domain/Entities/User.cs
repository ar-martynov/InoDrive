using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Entities
{
    [Table("AspNetUsers")]
    public class User:IdentityUser
    {
        private ICollection<Bid> _bids;
        private ICollection<Tour> _tours;
        private ICollection<Rating> _ratings;
        public User() { }

        public virtual UserProfile UserProfile { get; set; }
        
        public virtual ICollection<Bid> Bids
        {
            get { return _bids ?? (_bids = new List<Bid>()); }
            protected set { _bids = value; }
        }

        public virtual ICollection<Tour> Tours
        {
            get { return _tours ?? (_tours = new List<Tour>()); }
            protected set { _tours = value; }
        }

        public virtual ICollection<Rating> Ratings
        {
            get { return _ratings ?? (_ratings = new List<Rating>()); }
            protected set { _ratings = value; }
        }
    }
}
