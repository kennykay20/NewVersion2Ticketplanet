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
   

   public class UserProfileRepository : Repository<tk_UserProfile>, IUserProfileRepository
   {
       public UserProfileRepository(IDbFactory dbFactory)
           : base(dbFactory) { }
   }

   public interface IUserProfileRepository : IRepository<tk_UserProfile>
   {

   }
}
