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
   public class RefferenceGenerator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;
        private readonly IRoleAssignmentRepository repoRoleAssign;
        private readonly IRoleRepository repoRole;
       public RefferenceGenerator()
       {
           
           idbfactory = new DbFactory();
           unitOfWork = new UnitOfWork(idbfactory);
           repoRoleAssign = new RoleAssignmentRepository(idbfactory);
           repoRole = new RoleRepository(idbfactory);
       }
       public static string GenerateReference(int batchCount)
       {
           string RefNo = "";
           try
           {
            

           var rand = new Random();
           string randomNo = rand.Next(1234, 3241).ToString();
           string prefix = System.Configuration.ConfigurationManager.AppSettings["TicketPlanetPrefix"];
           RefNo = prefix + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + randomNo + batchCount.ToString();


           }
           catch (Exception)
           {

              
           }
           return RefNo;

       }

       public static string GenerateReferenceFLW(int batchCount)
       {
           string RefNo = "";
           try
           {

            
               var rand = new Random();
               string randomNo = rand.Next(1234, 3241).ToString();
               string prefix = System.Configuration.ConfigurationManager.AppSettings["TicketPlanetPrefix"];
               RefNo = prefix + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + randomNo + batchCount.ToString();


           }
           catch (Exception)
           {


           }
           return RefNo;

       }

       public string GetRoleName(int? roleId)
       {
           string RoleName = repoRole.GetNonAsync(c => c.RoleId == roleId).RoleName;

           return RoleName;
       }
    }
}
