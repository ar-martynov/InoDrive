using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Abstract.EntityRepositories
{
    public interface ICityRepository : IRepository<City>
    {
        WayPoint GetWayPoint(City newCity, Region newRegion);
        City GetCity(City newCity, Region newRegion);
    }
}
