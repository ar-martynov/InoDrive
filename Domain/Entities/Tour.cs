using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    public class Tour
    {
        private ICollection<Bid> _bids;
        private ICollection<Rating> _ratings;
        private ICollection<WayPoint> _wayPoints { get; set; }
        public int TourId { get; set; }
        public string UserId { get; set; }
        public int TotalSlots { get; set; }
        public int FreeSlots { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsDeleted { get; set; }

        [Column(TypeName = "Money")]
        public decimal Payment { get; set; }

       
        public virtual ICollection<Bid> Bids
        {
            get { return _bids ?? (_bids = new List<Bid>()); }
            protected set { _bids = value; }
        }
        public virtual ICollection<WayPoint> WayPoints 
        {
            get { return _wayPoints ?? (_wayPoints = new List<WayPoint>()); }
            protected set { _wayPoints = value; }
        }

        public virtual ICollection<Rating> Ratings
        {
            get { return _ratings ?? (_ratings = new List<Rating>()); }
            protected set { _ratings = value; }
        }
        
        public virtual User User { get; set; }
        //public virtual City StartCity { get; set; }
        //public virtual City DestCity { get; set; }
        public virtual TourProfile TourProfile { get; set;}
        
    }
}
