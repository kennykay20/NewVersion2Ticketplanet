using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    
    public class MaryLandFilmRepository : Repository<tk_GenesisFilmsMaryland>, IMaryLandFilmRepository
    {
        public MaryLandFilmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IMaryLandFilmRepository : IRepository<tk_GenesisFilmsMaryland>
    {

    }

}
