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
   

    public class GateWayFilmRepository : Repository<tk_GenesisFilmsGateway>, IGateWayFilmRepository
    {
        public GateWayFilmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IGateWayFilmRepository : IRepository<tk_GenesisFilmsGateway>
    {

    }
}
