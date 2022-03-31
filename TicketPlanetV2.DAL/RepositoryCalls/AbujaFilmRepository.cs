
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanet.DAL.Implementation;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    
    public class AbujaFilmRepository : Repository<tk_GenesisFilmsAbuja>, IAbujaFilmRepository
    {
        public AbujaFilmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
        
    }

    public interface IAbujaFilmRepository : IRepository<tk_GenesisFilmsAbuja>
    {

    }
}
