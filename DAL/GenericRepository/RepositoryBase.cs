using DAL.Models;
using DAL.Pagination;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


namespace DAL.Repository
{
    public class RepositoryBase : IRepositoryBase
    {
        public DualPracticeContext DualPracticeContext;
        private string connectionString { set; get; }

        public RepositoryBase(DualPracticeContext _DualPracticeContext, IConfiguration configuration)
        {
            DualPracticeContext = _DualPracticeContext;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        public RepositoryBase(DualPracticeContext _DualPracticeContext)
        {
            DualPracticeContext = _DualPracticeContext;
        }


        public async Task<T> CreateAsync<T>(T entity) where T : class
        {
            try
            {
                var returnedEntity = await DualPracticeContext.Set<T>().AddAsync(entity);
                await DualPracticeContext.SaveChangesAsync();
                return returnedEntity.Entity;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await DualPracticeContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        public void Delete<T>(T entity) where T : class
        {
            try
            {
                DualPracticeContext.Set<T>().Remove(entity);
                DualPracticeContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        public void Update<T>(T entity) where T : class
        {
            try
            {

                DualPracticeContext.Set<T>().Update(entity);
                DualPracticeContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //public async Task<IList<T>> ExecuteStoredProcedure(string ProcName, params object[] parameters)
        //{
        //    try
        //    {
        //        return await DualPracticeContext.Set<T>().FromSqlRaw(@"Exec " + ProcName, parameters).ToListAsync();

        //    }
        //    catch (HttpStatusCodeException e)
        //    {
        //        throw new HttpStatusCodeException(HttpStatusCode.NotFound, (ErrorCode.DatabaseError).ToString(), ErrorCode.DatabaseError);
        //    }
        //}



        public async Task<IList<T>> FindAllAsync<T>(bool trackchanges) where T : class
        {
            var result = !trackchanges ?
                                        await DualPracticeContext.Set<T>().AsNoTracking().ToListAsync()
                                      : await DualPracticeContext.Set<T>().ToListAsync();
            return result;
        }



        public async Task<IList<T>> FindByConditionAsync<T>(Expression<Func<T, bool>> where, bool trackchanges) where T : class
        {
            var DataWithoutTracking = !trackchanges ?
                await DualPracticeContext.Set<T>().Where(where).AsNoTracking().ToListAsync()
               : await DualPracticeContext.Set<T>().Where(where).ToListAsync();
            return DataWithoutTracking;
        }

        public Task<T> FirstAsync<T>(Expression<Func<T, bool>> predicate) where T : class
            => DualPracticeContext.Set<T>().FirstOrDefaultAsync(predicate);

        // stored procedures 
        public async Task<string> ExecuteUDFSingleValue(string ProcName, params SqlParameter[] sqlParameter)
        {

            using (var cmd = DualPracticeContext.Database.GetDbConnection().CreateCommand())
            {

                cmd.CommandText = "SELECT " + ProcName + "(" + string.Join(",", sqlParameter.Select(x => "@" + x.ParameterName)) + ")";
                cmd.Parameters.AddRange(sqlParameter);
                DualPracticeContext.Database.OpenConnection();

                var result = await cmd.ExecuteReaderAsync();

                await result.ReadAsync();
                try
                {
                    if (result.HasRows)
                    {
                        return result[0].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    await DualPracticeContext.Database.GetDbConnection().CloseAsync();
                }

            }

        }

        public DataTable ExecuteFromSqlRaw(string query)
        {
            using (var sqlconnection = new SqlConnection(DualPracticeContext.configuration.GetConnectionString("ConnectionStrings:DualPractice")))
            {
                sqlconnection.Open();
                using (var sqlcommand = new SqlCommand(query, sqlconnection))
                {
                    DataSet dataset = new DataSet();
                    SqlDataAdapter sqldataadaptor = new SqlDataAdapter(sqlcommand);
                    sqldataadaptor.Fill(dataset);

                    if (dataset.Tables.Count > 0)
                        return dataset.Tables[0];
                    else
                        return null;

                }
            }

        }

        public DataTable ExecuteStoredProcedure(string ProcName, params SqlParameter[] sqlParameter)
        {

            using (var sqlconnection = new SqlConnection(DualPracticeContext.configuration.GetConnectionString("ConnectionStrings:DualPractice")))
            {
                sqlconnection.Open();
                using (var sqlcommand = new SqlCommand(ProcName, sqlconnection))
                {
                    sqlcommand.CommandType = CommandType.StoredProcedure;
                    sqlcommand.Parameters.AddRange(sqlParameter);

                    DataSet dataset = new DataSet();
                    SqlDataAdapter sqldataadaptor = new SqlDataAdapter(sqlcommand);
                    sqldataadaptor.Fill(dataset);

                    if (dataset.Tables.Count > 0)
                        return dataset.Tables[0];
                    else
                        return new DataTable();
                }
            }
        }

        public async Task<int> ExecuteNonQuery(string sql, params SqlParameter[] sqlParameter) => await DualPracticeContext.Database.ExecuteSqlRawAsync(sql, sqlParameter);

        public Hashtable ExecuteStoredProcedureOutPut(string ProcName, params SqlParameter[] sqlParameter)
        {
            using (var sqlconnection = new SqlConnection(DualPracticeContext.configuration["ConnectionStrings:DefaultConnection"]))
            {
                sqlconnection.Open();
                using (var sqlcommand = new SqlCommand(ProcName, sqlconnection))
                {
                    sqlcommand.CommandType = CommandType.StoredProcedure;
                    sqlcommand.Parameters.AddRange(sqlParameter);
                    sqlcommand.ExecuteNonQuery();

                    Hashtable ht = new Hashtable();
                    foreach (SqlParameter item in sqlcommand.Parameters)
                        if (item.Direction == ParameterDirection.Output)
                            ht.Add(item.ParameterName, item.Value);
                    return ht;

                }
            }
        }

        public DataTable ExecuteUDFTable(string Name, params SqlParameter[] sqlParameter)
        {
            SqlConnection sqlconnection = new SqlConnection(DualPracticeContext.configuration["ConnectionStrings:DefaultConnection"]);
            using (var dataAdpt = new SqlDataAdapter(Name, sqlconnection))
            {
                foreach (SqlParameter item in sqlParameter)
                {
                    dataAdpt.SelectCommand.Parameters.Add(item);
                }
                var ds = new DataSet();
                dataAdpt.Fill(ds);
                return ds.Tables?[0] ?? null;
            }
        }

        public SqlDataReader GetDataReader(string stored_name, params SqlParameter[] par)
        {
            SqlConnection sqlconnection = new SqlConnection(connectionString);
            SqlCommand sqlcommand = new SqlCommand(stored_name, sqlconnection);
            sqlcommand.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter item in par)
            {
                sqlcommand.Parameters.Add(item);
            }
            sqlconnection.Open();
            SqlDataReader sqldatareader = sqlcommand.ExecuteReader();
            return sqldatareader;
        }

        public async Task<IReadOnlyCollection<T>> GetAsync<T>(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>,
                IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool tracingChanges = true) where T : class
        {
            IQueryable<T> query = DualPracticeContext.Set<T>();
            if (tracingChanges)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IList<T>> QueryJoinCondition<T>(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            var query = DualPracticeContext.Set<T>().AsQueryable();
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }
            if (where != null)
            {
                query = query.Where(where);
            }

            return await query.ToListAsync();
        }

        public async Task<IList<T>> TableListQuery<T>(
            Expression<Func<T, object>> includeProperties = null, 
            Expression<Func<T, bool>> where = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            PagedListInfo table = null) where T : class
        {
            var query = DualPracticeContext.Set<T>().AsQueryable();

            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }

            if (where != null)
            {
                query = query.Where(where);
            }
            
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            int count = query.Count();
            if (table.PageNumber >= 0 && table.PageSize > 0)
            {
                query = query.Skip(table.PageNumber * table.PageSize).Take(table.PageSize);
            }

            return await query.ToListAsync();
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public async Task<IList<T>> MultiInclude<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes) where T : class
        {
            DbSet<T> dbSet;
            dbSet = DualPracticeContext.Set<T>();

            IQueryable<T> query = dbSet;

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            //if (select != null)
            //    query = query.Select(select);
            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }
    }
}