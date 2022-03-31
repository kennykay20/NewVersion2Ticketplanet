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


   public class ClientServiceSetUpRepository : Repository<tk_ServiceSetUp>, IClientServiceSetUpRepository
   {
       public ClientServiceSetUpRepository(IDbFactory dbFactory)
           : base(dbFactory) { }
   }

   public interface IClientServiceSetUpRepository : IRepository<tk_ServiceSetUp>
   {

   }
}
