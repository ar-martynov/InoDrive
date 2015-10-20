using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Domain.Abstract;
using Domain.Abstract.EntityRepositories;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tours;
using Domain.Concrete.Repositories;

using WebUI.Infrastructure;

using AutoMapper;

namespace WebUI.Controllers
{
    [RoutePrefix("api/Tours")]
    [ApplicationExceptionFilter]
    public class ToursController : BaseApiController
    {
        public ToursController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        // POST Api/Tours/Create
        [Route("Create")]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult CreateTour([FromBody]TourModel tour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string uid = GetUserId();


            var tours = _UOW.GetRepository<ITourRepository>();
            var cities = _UOW.GetRepository<ICityRepository>();

            var newTour = Mapper.Map<TourModel, Tour>(tour);

            var user = ValidateUser(uid);

            for (int i = 0; i < tour.WayPoints.Count(); i++)
            {
                WayPoint wp = cities.GetWayPoint(Mapper.Map<CityModel, City>(tour.WayPoints[i]), Mapper.Map<CityModel, Region>(tour.WayPoints[i]));
                
                if (i == 0) wp.IsStart = true;
                if (i == tour.WayPoints.Count() - 1) wp.IsDestination = true;
                wp.WayPointOrder = i;
                newTour.WayPoints.Add(wp);
            }
           
            tours.CreateTour(user, newTour);
            _UOW.Commit();

            return Json(new { success = true, id = newTour.TourId, message = Resources.Language.TourCreated });
        }

        // Delete Api/Tours/:id
        [HttpDelete]
        [ApplicationAuthorize]
        public IHttpActionResult Delete(int id)
        {
            var tours = _UOW.GetRepository<ITourRepository>();
            var user = ValidateUser(GetUserId());
            var tour = ValidateUserTour(user, id);

            tours.DeleteTour(tour);
            _UOW.Commit();

            return Json(new { message = Resources.Language.TourDeleted });
        }


        // Get Api/Tours/Modify/:id
        [HttpGet]
        [Route("Modify")]
        [ApplicationAuthorize]
        public IHttpActionResult ModifyTour(int id)
        {
            var tours = _UOW.GetRepository<ITourRepository>();

            var user = ValidateUser(GetUserId());
            var tour = ValidateUserTour(user, id);
          
            ValidateTourExpiration(tour);
            var request = Mapper.Map<Tour, TourModel>(tour);

            return Ok(request);
        }


        // Put Api/Tours/Modify
        [HttpPut]
        [Route("Modify")]
        [ApplicationAuthorize]
        public IHttpActionResult ModifyTour([FromBody]TourModel tour)
        {
            var tours = _UOW.GetRepository<ITourRepository>();

            if (tour == null || tour.Id == 0) return BadRequest(Resources.Language.TourNotExist);

            var user = ValidateUser(GetUserId());
            var originTour = ValidateUserTour(user, tour.Id);
            ValidateTourExpiration(originTour);

            var updatedTour = Mapper.Map<TourModel, Tour>(tour);
            updatedTour.TourProfile.TourId = updatedTour.TourId;
            updatedTour.UserId = GetUserId();

            var cities = _UOW.GetRepository<ICityRepository>();

            for (int i = 0; i < tour.WayPoints.Count(); i++)
            {
                var city = Mapper.Map<CityModel, City>(tour.WayPoints[i]);
                var region = Mapper.Map<CityModel, Region>(tour.WayPoints[i]);
                WayPoint wp = cities.GetWayPoint(city, region);
                if (i == 0) wp.IsStart = true;
                if (i == tour.WayPoints.Count() - 1) wp.IsDestination = true;
                wp.WayPointOrder = i;
                updatedTour.WayPoints.Add(wp);
            }
            tours.ModifyTour(originTour, updatedTour);

            _UOW.Commit();

            return Json(new { success = true, message = Resources.Language.TourUpdated });
        }


        // GET Api/Tours/MyTours/:page
        [HttpGet]
        [Route("MyTours")]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult GetUserTours(int page)
        {
            var tours = _UOW.GetRepository<ITourRepository>();

            page = (page <= 0) ? 0 : page - 1;
            int totalPages = 0;

            var user = ValidateUser(GetUserId());

            var result = Mapper.Map<IEnumerable<Tour>, List<TourSearchResultModel>>(tours.GetUserTours(user, page, ConfigModel.PerPage, out totalPages));

           
            return Json(new { success = true, tours = result, totalPages = totalPages, page = page + 1 },
                        new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });
        }

        // POST Api/Tours/Search
        [HttpPost]
        [Route("Search")]
        [AllowAnonymous]
        public IHttpActionResult SearchTour([FromBody]TourSearchModel tour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tours = _UOW.GetRepository<ITourRepository>();
            var cities = _UOW.GetRepository<ICityRepository>();
            var tourToSearch = Mapper.Map<TourSearchModel, Tour>(tour);

            int startId = cities.GetCity(Mapper.Map<CityModel, City>(tour.StartCity), Mapper.Map<CityModel, Region>(tour.StartCity)).CityId;
            int destId = cities.GetCity(Mapper.Map<CityModel, City>(tour.DestCity), Mapper.Map<CityModel, Region>(tour.DestCity)).CityId;

            if (startId == 0 || destId == 0) return Json(new { success = false, message = Resources.Language.SearchNoResults });

            int totalPages;
            int page = (tour.Page == 0) ? 0 : tour.Page - 1;
            var result = Mapper.Map<IEnumerable<Tour>, List<TourSearchResultModel>>(tours.SearchTours(tourToSearch, startId, destId, page, ConfigModel.PerPage, out totalPages));

           
            return Json(new { success = true, tours = result, totalPages = totalPages, page = tour.Page },
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }

        // POST Api/Tours/AdvancedSearch
        [Route("AdvancedSearch")]
        [AllowAnonymous]
        public IHttpActionResult AdvancedSearchTour([FromBody]TourAdvancedSearchModel tour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tours = _UOW.GetRepository<ITourRepository>();
            var cities = _UOW.GetRepository<ICityRepository>();
            var tourToSearch = Mapper.Map<TourAdvancedSearchModel, Tour>(tour);

            int startId = cities.GetCity(Mapper.Map<CityModel, City>(tour.StartCity), Mapper.Map<CityModel, Region>(tour.StartCity)).CityId;
            int destId = cities.GetCity(Mapper.Map<CityModel, City>(tour.DestCity), Mapper.Map<CityModel, Region>(tour.DestCity)).CityId;

            if (startId == 0 || destId == 0) return Json(new { success = false, message = Resources.Language.SearchNoResults });

            int totalPages;
            int page = (tour.Page <= 0) ? 0 : tour.Page - 1;

            for (int i = 0; i < tour.WayPoints.Count(); i++)
            {
                WayPoint wp = cities.GetWayPoint(Mapper.Map<CityModel, City>(tour.WayPoints[i]), Mapper.Map<CityModel, Region>(tour.WayPoints[i]));
                wp.WayPointOrder = i;
                tourToSearch.WayPoints.Add(wp);
            }
            var result = Mapper.Map<IEnumerable<Tour>, List<TourSearchResultModel>>(tours.AdvancedSearchTours(
                tourToSearch,
                startId, destId,
                tour.AdvancedSearchProfile.PaymentLowerBound,
                tour.AdvancedSearchProfile.PaymentUpperBound, page, ConfigModel.PerPage, out totalPages));

            
            return Json(new { success = true, tours = result, totalPages = totalPages, page = tour.Page },
                        new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });
        }

        // Get Api/Tours/Get/:id
        [Route("Get")]
        [AllowAnonymous]
        public IHttpActionResult GetTourById(int id)
        {
            var tours = _UOW.GetRepository<ITourRepository>();
            var tour = ValidateTour(id);
            var model = Mapper.Map<Tour, TourModelWithOwner>(tour);

            if (RequestContext.Principal.Identity.IsAuthenticated)
            {
                string uid = GetUserId();
                if (uid == tour.UserId) model.IsOwner = true;
                if (tour.Bids.FirstOrDefault(x => x.UserId == uid) != null) model.AlreadyBet = true;
                
            }

           
            return Ok(model);
        }


        // GET Api/Tours/LastProfile
        [HttpGet]
        [Route("LastProfile")]
        [ApplicationAuthorizeAttribute]
        public IHttpActionResult GetLastTourProfile()
        {
            var tours = _UOW.GetRepository<ITourRepository>();
            var user = ValidateUser(GetUserId());
            var request = Mapper.Map<TourProfile, TourProfileModel>(tours.GetLastTourProfile(user));
           
            return Ok(request);
        }
    }
}
