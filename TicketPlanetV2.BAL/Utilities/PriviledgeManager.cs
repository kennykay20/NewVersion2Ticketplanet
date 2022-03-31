using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanet.DAL.Implementation;
using TicketPlanet.DAL.RepositoryCalls;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanetV2.BAL.Utilities
{
    public class PriviledgeAssignmentManager
    {
        public bool CanEdit { get; set; }
        public bool CanAdd { get; set; }
        public bool CanAuth { get; set; }
        public bool CanView { get; set; }

        
    }
    public class PriviledgeManager
    {
        private int _MenuId;
        private int _RoleId;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        private readonly IRoleAssignmentRepository repoRoleAssign;
        private readonly IRoleRepository repoRole;
        private PriviledgeAssignmentManager primanager;

        public PriviledgeManager(int MenuId, int RoleId)
        {
            _MenuId = MenuId;
            _RoleId = RoleId;
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoRoleAssign = new RoleAssignmentRepository(idbfactory);
            repoRole = new RoleRepository(idbfactory);
        }
        public PriviledgeAssignmentManager AssignRoleToUser()
        {
            primanager = new PriviledgeAssignmentManager();

            var d = repoRoleAssign.GetNonAsync(p => p.MenuId == _MenuId && p.RoleId == _RoleId);
            if (d != null)
            {
                primanager.CanAdd = d.CanAdd == 1 ? true : false;
                primanager.CanEdit = d.CanEdit == 1 ? true : false;
                primanager.CanAuth = d.CanAuth == 1 ? true : false;
                primanager.CanView = d.CanView == 1 ? true : false;
                return primanager;
            }
            return null;
        }

        public bool CanAuth()
        {
            throw new NotImplementedException();
        }

        public bool CanEdit()
        {
            throw new NotImplementedException();
        }
       

       

    }
}
