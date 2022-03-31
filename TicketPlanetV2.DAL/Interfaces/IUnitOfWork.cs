using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPlanetV2.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> Commit(int userId);
        int CommitNonAsync(int userId);
    }
}
