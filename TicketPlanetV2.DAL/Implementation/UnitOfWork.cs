using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Context;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanetV2.DAL.Implementation
{
  

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private TicketPlanetContext dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public TicketPlanetContext DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public async Task<int> Commit(int userId)
        {

            return await DbContext.Commit(userId);
        }

        public int CommitNonAsync(int userId)
        {

            return DbContext.CommitNonAsync(userId);
        }
    }
}

