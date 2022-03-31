using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    

    public class Luxury51SeaterRepository : Repository<tk_Luxury51Seater>, ILuxury51SeaterRepository
    {
        public Luxury51SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ILuxury51SeaterRepository : IRepository<tk_Luxury51Seater>
    {

    }
}
