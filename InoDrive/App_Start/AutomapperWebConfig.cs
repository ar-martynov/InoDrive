using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Tours;

namespace WebUI
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            #region Users

            Mapper.CreateMap<UserRegistrationModel, User>()
                .ForMember(x => x.Email, opts => opts.MapFrom(src => src.UserName));
            Mapper.CreateMap<UserRegistrationModel, UserProfile>();
            Mapper.CreateMap<UserProfile, UserProfileModel>();
            Mapper.CreateMap<UserProfileModel, UserProfile>();
            Mapper.CreateMap<UserProfile, UserRatedProfileModel>();
          
            
          

            #endregion


            #region Tours

            Mapper.CreateMap<TourModel, Tour>()
                .ForMember(x => x.CreationDate, opts => opts.MapFrom(src => DateTime.Now))
                .ForMember(x => x.ExpirationDate, opts => opts
                    .MapFrom(src => DateTime.ParseExact(src.ExpirationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(x => x.TotalSlots, opts => opts.MapFrom(src => src.FreeSlots))
                .ForMember(x => x.WayPoints, opts => opts.Ignore())
                .ForMember(x => x.Payment, opts => opts.MapFrom( src => src.Payment == null ? 0 : (decimal)src.Payment))
                .ForMember(x => x.TourId, opts => opts.MapFrom(src => src.Id));

            Mapper.CreateMap<Tour, TourModel>()
                .ForMember(x => x.WayPoints, opt => opt
                    .MapFrom(src => src.WayPoints.Select(wp => new CityModel { CityNameRu = wp.City.CityNameRu, 
                                                                               RegionNameRu = wp.City.Regions.FirstOrDefault().RegionNameRu,
                                                                               Latitude = wp.City.Latitude, 
                                                                               Longtitude = wp.City.Longtitude})))
                .ForMember(x => x.ExpirationDate, opt => opt
                    .MapFrom(src => src.ExpirationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(x => x.Id, opt => opt
                    .MapFrom(src => src.TourId));

            Mapper.CreateMap<Tour, TourSearchResultModel>()
                .ForMember(x => x.IsExpired, opts => opts.MapFrom(src => src.ExpirationDate < DateTime.Now))
                .ForMember(x => x.Id, opts => opts.MapFrom(src => src.TourId))
                .ForMember(x => x.Owner, opts => opts
                    .MapFrom(src => src.User.UserProfile.FirstName + " " + src.User.UserProfile.LastName))
                .ForMember(x => x.Payment, opts => opts
                    .MapFrom(src => src.Payment == 0 || src.Payment == 0 ? "договорная" : String.Format("{0:0,0}", src.Payment) + " р."))
                .ForMember(x => x.ExpirationDate, opts => opts
                    .MapFrom(src => src.ExpirationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(x => x.StartCity, opts => opts.MapFrom(src => src.WayPoints.FirstOrDefault(wp => wp.IsStart == true).City.CityNameRu))
                .ForMember(x => x.DestCity, opts => opts.MapFrom(src => src.WayPoints.FirstOrDefault(wp => wp.IsDestination == true).City.CityNameRu));       

            Mapper.CreateMap<Tour, TourModelWithOwner>()
                .ForMember(x => x.UserProfile, opts => opts.MapFrom(src => src.User.UserProfile))
                .ForMember(x => x.IsExpired, opts => opts.MapFrom(src => src.ExpirationDate < DateTime.Now))
                .ForMember(x => x.Payment, opts => opts
                    .MapFrom(src => src.Payment == 0 || src.Payment == 0 ? "договорная" : String.Format("{0:0,0}", src.Payment) + " р."))
                .ForMember(x => x.WayPoints, opts => opts
                    .MapFrom(src => src.WayPoints.OrderBy(city=>city.WayPointOrder)
                        .Select(wp => new City { CityNameRu = wp.City.CityNameRu, 
                                                      Latitude = wp.City.Latitude,
                                                      Longtitude = wp.City.Longtitude}).ToList()))
                .ForMember(x => x.ExpirationDate, opts => opts
                    .MapFrom(src => src.ExpirationDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)));
            
            Mapper.CreateMap<TourProfileModel, TourProfile>();
            Mapper.CreateMap<TourProfileSearchModel, TourProfile>();
            Mapper.CreateMap<TourProfile, TourProfileModel>();

            Mapper.CreateMap<TourSearchModel, Tour>()
                .ForMember(x => x.ExpirationDate, opts => opts
                    .MapFrom(src => DateTime.ParseExact(src.ExpirationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)));

            Mapper.CreateMap<TourAdvancedSearchModel, Tour>()
                 .ForMember(x => x.TourProfile, opts => opts
                    .MapFrom(src => src.AdvancedSearchProfile))
                .ForMember(x => x.ExpirationDate, opts => opts
                    .MapFrom(src => DateTime.ParseExact(src.ExpirationDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(x=>x.WayPoints, opts => opts.Ignore());

            #endregion
                    

            #region Cities

            Mapper.CreateMap<CityModel, City>();
            Mapper.CreateMap<CityModel, Region>();
            Mapper.CreateMap<City, CityModel>()
                .ForMember(x => x.RegionNameRu, opts => opts.MapFrom(src => src.Regions));
            #endregion


            #region Bids
            
            Mapper.CreateMap<Bid, BidModel>()
                .ForMember(x=> x.User, opts => opts.MapFrom(src => src.User.UserProfile))
                .ForMember(x=> x.IsTourCompleted, opts => opts.MapFrom(src => src.Tour.IsDeleted))
                .ForMember(x => x.Id, opts => opts.MapFrom(src => src.BidId));

            Mapper.CreateMap<Bid, BidModelWithoutOwnerProfile>()
                .ForMember(x=> x.IsTourCompleted, opts => opts.MapFrom(src => src.Tour.IsDeleted))
                .ForMember(x => x.Id, opts => opts.MapFrom(src => src.BidId));

            #endregion

            #region SignalRMessages
            Mapper.CreateMap<Tour, NewBidNotifyModel>()
                .ForMember(x => x.UserIdentityId, opts => opts.MapFrom(src => src.User.UserName))
                .ForMember(x => x.TourName, opts => opts
                    .MapFrom(src => String.Format("{0} - {1}", src.WayPoints.FirstOrDefault(wp => wp.IsStart).City.CityNameRu,
                                                               src.WayPoints.FirstOrDefault(wp => wp.IsDestination).City.CityNameRu)));
            #endregion
        }

    }
}
