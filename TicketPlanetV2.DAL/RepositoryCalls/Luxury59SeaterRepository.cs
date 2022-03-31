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


    public class Luxury59SeaterRepository: Repository<tk_Luxury59Seater>, ILuxury59SeaterRepository
    {
        public Luxury59SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ILuxury59SeaterRepository : IRepository<tk_Luxury59Seater>
    {

    }
}
