using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketPlanetV2.DAL.Context;

namespace TicketPlanetV2.DAL.Interfaces
{
    public interface IDbFactory : IDisposable
    {
        TicketPlanetContext Init();
    }
}
