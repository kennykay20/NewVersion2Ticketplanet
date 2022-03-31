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


    public class MiniLuxury25SeaterRepository: Repository<tk_MiniLuxury25Seater>, IMiniLuxury25SeaterRepository
    {
        public MiniLuxury25SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IMiniLuxury25SeaterRepository : IRepository<tk_MiniLuxury25Seater>
    {

    }
}
