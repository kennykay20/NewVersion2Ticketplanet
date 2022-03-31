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


    public class MiniLuxury33SeaterRepository : Repository<tk_MiniLuxury33Seater>, IMiniLuxury33SeaterRepository
    {
        public MiniLuxury33SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IMiniLuxury33SeaterRepository : IRepository<tk_MiniLuxury33Seater>
    {

    }
}
