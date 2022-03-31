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
    
    public class AsabaFilmRepository : Repository<tk_GenesisFilmsAsaba>, IAsabaFilmRepository
    {
        public AsabaFilmRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IAsabaFilmRepository : IRepository<tk_GenesisFilmsAsaba>
    {

    }
}
