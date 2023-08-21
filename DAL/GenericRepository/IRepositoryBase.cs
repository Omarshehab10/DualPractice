using DAL.Pagination;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repository
{

    public interface IRepositoryBase
    {
        Task<T> CreateAsync<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<IList<T>> FindAllAsync<T>(bool trackchanges) where T : class;
        Task<IList<T>> FindByConditionAsync<T>(Expression<Func<T, bool>> where, bool trackchanges) where T : class;
        Task<T> FirstAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<IReadOnlyCollection<T>> GetAsync<T>(
                Expression<Func<T, bool>> predicate = null,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                List<Expression<Func<T, object>>> includes = null,
                bool tracingChanges = true) where T : class;
        Task<IList<T>> MultiInclude<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes) where T : class;
        Task<IList<T>> QueryJoinCondition<T>(Expression<Func<T, bool>> where = null, params Expression<Func<T, object>>[] includeProperties) where T : class;
        Task<int> SaveAsync();
        Task<IList<T>> TableListQuery<T>(
            Expression<Func<T, object>> includeProperties = null,
            Expression<Func<T, bool>> where = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            PagedListInfo table = null
            ) where T : class;


        // ******** Procedure 
        string GetConnectionString();
        Task<string> ExecuteUDFSingleValue(string ProcName, params SqlParameter[] sqlParameter);
        DataTable ExecuteFromSqlRaw(string query);
        DataTable ExecuteStoredProcedure(string ProcName, params SqlParameter[] sqlParameter);

        public Task<int> ExecuteNonQuery(string sql, params SqlParameter[] sqlParameter);

        public Hashtable ExecuteStoredProcedureOutPut(string stored_name, params SqlParameter[] par);

        public DataTable ExecuteUDFTable(string Name, params SqlParameter[] sqlParameter);

        SqlDataReader GetDataReader(string stored_name,  params SqlParameter[] par);
    }
}