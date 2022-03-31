using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Context;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanetV2.DAL.Implementation
{

    public abstract class Repository<T> where T : class
    {
        #region Properties
        private TicketPlanetContext dataContext;
        private readonly DbSet<T> dbSet;

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        //IDbFactory db = new DbFactory();

        protected TicketPlanetContext DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }
        #endregion

        protected Repository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            dbSet = DbContext.Set<T>();

        }

        #region Implementation
        public virtual void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            //dataContext.Configuration.AutoDetectChangesEnabled = false;
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;

        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {

            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
        }

        public virtual T GetById(object id)
        {
            return dbSet.Find(id);
        }
        public virtual IEnumerable<T> GetAllNonAsync()
        {
            return dbSet.ToList();
        }

        public async virtual Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async virtual Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> where)
        {
            return await dbSet.Where(where).ToListAsync();
        }
        public virtual IEnumerable<T> GetManyNonAsync(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public async Task<T> Get(Expression<Func<T, bool>> where)
        {
            if (where != null)
                return await dbSet.Where(where).FirstOrDefaultAsync<T>();
            else
                return await dbSet.SingleOrDefaultAsync();
        }
        public T GetNonAsync(Expression<Func<T, bool>> where)
        {
            if (where != null)
                return dbSet.Where(where).FirstOrDefault<T>();
            else
                return dbSet.SingleOrDefault();
        }

        public virtual IEnumerable<T> ExecuteStoredProcedure(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters);

        }

        #endregion



        /* //bSet<T> dbset;
         private readonly ZenithBankIBContext context;
         public Repository(IUnitOfWork uow)
         {
             context = uow.Context as ZenithBankIBContext;
         }
         public IEnumerable<T> All
         {
             get
             {
                 return context.Set<T>();
             }
         }
         public IEnumerable<T> AllEager(params Expression<Func<T, object>>[] includes)
         {
             IQueryable<T> query = context.Set<T>();
             //foreach (var include in includes)
             //{
             //    query = query.Include(include);
             //}
             return query;
         }
         public T Find(object id)
         {
             return context.Set<T>().Find(id);
         }
         public void Insert(T item)
         {
             try
             {
                 context.Entry(item).State = EntityState.Added;
             }
             catch (Exception ex)
             {

             }

             // dynamic obj = context.Set<T>().Add(item);


         }

         /// <summary>
         /// Update The Entity in the DbContext
         /// </summary>
         /// <param name="item"></param>
         public void Update(T item)
         {
             context.Set<T>().Attach(item);
             context.Entry(item).State = EntityState.Modified;
         }
         public void Delete(int id)
         {
             var item = context.Set<T>().Find(id);
             //context.Set<T>().Remove(item);
             context.Entry(item).State = EntityState.Deleted;
         }
         public IEnumerable<T> LoadViaStockProc(string procName, params object[] param) // SqlParameter param)// params object[] param)
         {

             // new ObjectParameter("roleid", typeof(int));
             //string val = string.Empty;
             //foreach (var h in param)
             //{
             //    val += h + ",";
             //}

             //  var g = val.TrimEnd(new char[]{Convert.ToChar(","),Convert.ToChar(",")}) ;
             //user.TrimEnd(New Char() {","c, " "c, ControlChars.Lf})
             IEnumerable<T> res = context.Database.SqlQuery<T>(procName);
             return res;
             // get
             //{
             //    return context.Database.SqlQuery<T>("EXEC Test");
             //}
         }

         //public virtual List<rolemenu_Result> rolemenu(Nullable<int> roleid, Nullable<int> coyid)
         //{
         //    var roleidParameter = roleid.HasValue ?
         //        new ObjectParameter("roleid", roleid) :
         //        new ObjectParameter("roleid", typeof(int));

         //    var coyidParameter = coyid.HasValue ?
         //        new ObjectParameter("coyid", coyid) :
         //        new ObjectParameter("coyid", typeof(int));

         //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<rolemenu_Result>("rolemenu", roleidParameter, coyidParameter).ToList();
         //}
         public void Dispose()
         {
             if (context != null)
                 context.Dispose();
         }*/
    }
}
