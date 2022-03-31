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
   
    public class UserLoginRepository : Repository<tk_UserLogin>, IUserLoginRepository
    {
        public UserLoginRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IUserLoginRepository : IRepository<tk_UserLogin>
    {

    }
}
