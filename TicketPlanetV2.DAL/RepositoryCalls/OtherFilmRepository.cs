﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    public class OtherFilmRepository : Repository<tk_FilmHouse_Film>, IOtherFilmRepository
    {
        public OtherFilmRepository(IDbFactory dbFactory)
            : base(dbFactory)
        { }
    }

    public interface IOtherFilmRepository : IRepository<tk_FilmHouse_Film>
    {

    }
}
