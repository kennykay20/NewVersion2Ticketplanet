using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanetV2.DAL.Context
{
    public partial class TicketPlanetContext : DbContext, IUnitOfWork
    {
        static TicketPlanetContext()
        {
            Database.SetInitializer<TicketPlanetContext>(null);
        }
        public TicketPlanetContext()
            : base("TicketPlanetEntities")
        {

            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{

        //    throw new UnintentionalCodeFirstException();
        //}

        public DbSet<tk_DBAudit> tk_DBAudit { get; set; }
        public override int SaveChanges()
        {
            throw new InvalidOperationException("User ID must be provided");
        }
        private List<tk_DBAudit> GetAuditRecordsForChange(DbEntityEntry dbEntry, int userId)
        {
            List<tk_DBAudit> result = new List<tk_DBAudit>();
            try
            {



                DateTime changeTime = DateTime.Now;

                TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault() as TableAttribute;

                string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

                var keyNames = dbEntry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).ToList();

                string keyName = keyNames[0].Name;
                if (dbEntry.State == System.Data.Entity.EntityState.Deleted)
                {
                    result.Add(new tk_DBAudit()
                    {
                        auditlogid = Guid.NewGuid(),
                        userid = userId.ToString(),
                        eventdateutc = changeTime,
                        eventtype = "D", // Deleted
                        tablename = tableName,
                        recordid = dbEntry.GetDatabaseValues().GetValue<object>(keyName).ToString(),
                        columnname = "*ALL",
                        newvalue = "yes" // (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                    }
                        );
                }
                else if (dbEntry.State == System.Data.Entity.EntityState.Modified)
                {
                    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                    {
                        //  For updates, we only want to capture the columns that actually changed
                        //if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                        //{
                        var gf = dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString();
                        var ga = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
                        if (gf != ga)
                        {
                            result.Add(new tk_DBAudit()
                            {
                                auditlogid = Guid.NewGuid(),
                                userid = userId.ToString(),
                                eventdateutc = changeTime,
                                eventtype = "M",    // Modified
                                tablename = tableName,
                                recordid = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                columnname = propertyName,
                                originalvalue = dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString(),
                                newvalue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                            }
                                );
                        }
                        // }
                    }
                }
                //  Otherwise, don't do anything, we don't care about Unchanged or Detached entities


            }
            catch
            {
            }

            return result;
        }

        public async Task<int> Commit(int userId)
        {
            try
            {

                // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.Entity.EntityState.Deleted || p.State == System.Data.Entity.EntityState.Modified))
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (tk_DBAudit x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.tk_DBAudit.Add(x);
                    }
                }

                // var gh = base.SaveChanges();
                // return gh;
                // Call the original SaveChanges(), which will save both the changes made and the audit records
                return await base.SaveChangesAsync();




            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                    ); // Add the original exception as the innerException
            }

            return 0;
        }

        public int CommitNonAsync(int userId)
        {
            try
            {

                // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.Entity.EntityState.Deleted || p.State == System.Data.Entity.EntityState.Modified))
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (tk_DBAudit x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.tk_DBAudit.Add(x);
                    }
                }

                // var gh = base.SaveChanges();
                // return gh;
                // Call the original SaveChanges(), which will save both the changes made and the audit records
                return base.SaveChanges();




            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                    ); // Add the original exception as the innerException
            }

            return 0;
        }


    }


 
}
