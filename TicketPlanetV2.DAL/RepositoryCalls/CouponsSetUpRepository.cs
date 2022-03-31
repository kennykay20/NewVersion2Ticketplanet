using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    
    public class CouponsSetUpRepository : Repository<tk_CouponSetUp>, ICouponsSetUpRepository
    {
        public CouponsSetUpRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ICouponsSetUpRepository : IRepository<tk_CouponSetUp>
    {

    }
}
