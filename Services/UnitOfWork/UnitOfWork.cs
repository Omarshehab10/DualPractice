using Common.Types;
using DAL.Models;
using DAL.Repository;
using DTO.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        // i have seen the implementation for this example from here
        // https://github.com/SaiGonSoftware/Awesome-CMS-Core/blob/575e8c8f3ba3daf7102d63703d926a6c83714ba0/src/AwesomeCMSCore/Modules/AwesomeCMSCore.Modules.Admin/Repositories/PostRepository.cs#L18
        private readonly DualPracticeContext db;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public Dictionary<Type, object> Repositories;
        public UnitOfWork(DualPracticeContext db, ILogger<IUnitOfWork> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.db = db;
            Repositories = new Dictionary<Type, object>();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public virtual IGenericRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepository<T>;
            }

            IGenericRepository<T> repo = new GenericRepository<T>(db);

            Repositories.Add(typeof(T), repo);

            return repo;
        }

        public async Task SaveChangesAsync()
        {
            _httpContextAccessor.HttpContext = null;

            string userId = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            //bool isLogEnabled = Convert.ToBoolean(_configuration["App:IsLogEnabled"]);

            //if (isLogEnabled)
            //    await OnBeforeSaveChanges(userId);


            await db.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            foreach (var entry in db.ChangeTracker.Entries().ToList())
            {
                await db.Entry(entry).ReloadAsync();
            }
        }

        public IQueryable<T> ApplySort<T>(IQueryable<T> entities, IList<OrderingParameters> OrderingParameters) where T : class
        {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            bool HasbeenOrderedBefore = false;

            foreach (var param in OrderingParameters)
            {
                if (string.IsNullOrWhiteSpace(param.OrderField))
                    continue;

                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(param.OrderField, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                entities = OrderingHelper<T>(entities, objectProperty.Name, param.IsDesc, HasbeenOrderedBefore);

                HasbeenOrderedBefore = true;
            }

            return entities;
        }

        private IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty);

            MemberExpression property = Expression.PropertyOrField(param, propertyName);

            LambdaExpression sort = Expression.Lambda(property, param);

            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        public void SaveChanges()
        {
            _httpContextAccessor.HttpContext = null;

            string userId = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            //bool isLogEnabled = Convert.ToBoolean(_configuration["App:IsLogEnabled"]);

            //if (isLogEnabled)
            //    await OnBeforeSaveChanges(userId);


             db.SaveChanges();
        }

        //private async Task OnBeforeSaveChanges(string userId)
        //{
        //    db.ChangeTracker.DetectChanges();

        //    var auditEntries = new List<AuditEntry>();

        //    foreach (var entry in db.ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //            continue;

        //        var auditEntry = new AuditEntry(entry);

        //        auditEntry.TableName = entry.Entity.GetType().Name;

        //        auditEntry.UserId = userId;

        //        auditEntries.Add(auditEntry);

        //        foreach (var property in entry.Properties)
        //        {
        //            string propertyName = property.Metadata.Name;

        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[propertyName] = property.CurrentValue;
        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:
        //                    auditEntry.AuditType = AuditType.Create;
        //                    auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    break;
        //                case EntityState.Deleted:
        //                    auditEntry.AuditType = AuditType.Delete;
        //                    auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                    break;
        //                case EntityState.Modified:
        //                    if (property.IsModified)
        //                    {
        //                        auditEntry.ChangedColumns.Add(propertyName);
        //                        auditEntry.AuditType = AuditType.Update;
        //                        auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                        auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    }
        //                    break;
        //            }
        //        }
        //    }

        //    foreach (var auditEntry in auditEntries)
        //    {
        //        await db.Audit.AddAsync(auditEntry.ToAudit());
        //    }
        //}
    }
}
