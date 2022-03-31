using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    
    public class OwerriFilmRepository : Repository<tk_GenesisFilmsOwerri>, IOwerriFilmRepository
    {
        public OwerriFilmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IOwerriFilmRepository : IRepository<tk_GenesisFilmsOwerri>
    {

    }
}
