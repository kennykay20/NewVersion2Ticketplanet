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


    public class Hummer15SeaterRepository : Repository<tk_Hummer15Seater>, IHummer15SeaterRepository
    {
        public Hummer15SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IHummer15SeaterRepository : IRepository<tk_Hummer15Seater>
    {

    }
}
