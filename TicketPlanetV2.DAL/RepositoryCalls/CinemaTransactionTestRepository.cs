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
    public class CinemaTransactionTestRepository : Repository<tk_CinemaTransactionLog>, ICinemaTransactionLogTestRepository
    {
        public CinemaTransactionTestRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ICinemaTransactionLogTestRepository : IRepository<tk_CinemaTransactionLog>
    {

    }
}
