using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

using Domain.Contexts;
using Domain.Models;
using Domain.Entities;
using Domain.Abstract;
using Domain.Abstract.EntityRepositories;

namespace Domain.Concrete.Repositories
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(InoDriveContext context)
            : base(context) { }
        

        /// <summary>
        /// Checks the city.
        /// </summary>
        /// <param name="newCity">The new city.</param>
        /// <param name="newRegion">The new region.</param>
        /// <returns></returns>
        private City CheckCity(City newCity, Region newRegion)
        {

            Region region = _context.Regions
                .FirstOrDefault(x => x.RegionNameRu != null && x.RegionNameRu == newRegion.RegionNameRu || x.RegionNameEn != null && x.RegionNameEn == newRegion.RegionNameEn);

            if (region == null)
            {
                region = _context.Regions.Add(newRegion);
                _context.SaveChanges();
            }

            City city = region.Cities.FirstOrDefault(x => x.Latitude == newCity.Latitude && x.Longtitude == newCity.Longtitude
                    && (x.CityNameRu != null && x.CityNameRu == newCity.CityNameRu || x.CityNameEn != null && x.CityNameEn == newCity.CityNameEn));

            if (city == null)
            {
                city = _context.Cities.Add(newCity);
                region.Cities.Add(city);
                _context.SaveChanges();
            }
            return city;

        }

        /// <summary>
        /// Gets the way point.
        /// </summary>
        /// <param name="newCity">The new city.</param>
        /// <param name="newRegion">The new region.</param>
        /// <returns></returns>
        public WayPoint GetWayPoint(City newCity, Region newRegion)
        {
            City city = CheckCity(newCity, newRegion);
            return new WayPoint
            {
                City = city,
                CityId = city.CityId,
            };

        }

        /// <summary>
        /// Gets the city.
        /// </summary>
        /// <param name="newCity">The new city.</param>
        /// <param name="newRegion">The new region.</param>
        /// <returns></returns>
        public City GetCity(City newCity, Region newRegion)
        {
            return CheckCity(newCity, newRegion);
        }
    }
}
