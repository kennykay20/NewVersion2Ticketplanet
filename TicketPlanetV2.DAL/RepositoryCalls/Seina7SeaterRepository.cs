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


    public class Seina7SeaterRepository : Repository<tk_Seina7Seater>, ISeina7SeaterRepository
    {
        public Seina7SeaterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface ISeina7SeaterRepository : IRepository<tk_Seina7Seater>
    {

    }
}
