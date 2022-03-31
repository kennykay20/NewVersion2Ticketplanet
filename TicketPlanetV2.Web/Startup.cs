using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;

using System.Configuration;
using Hangfire.SqlServer;
using TicketPlanetV2.BAL.EventModel;

namespace TicketPlanetV2.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            GlobalConfiguration.Configuration
            .UseSqlServerStorage("DefaultConnection", new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(2),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(2),
                QueuePollInterval = TimeSpan.Zero,

                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            }); app.UseHangfireDashboard();

            app.UseHangfireServer();
            EventClassModel events = new EventClassModel();
            RecurringJob.AddOrUpdate(() => events.SearchAllEventEmails(), "0 0 */30 * *");
        }
    }
}