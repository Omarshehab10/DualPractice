using Common.Types;
using DAL.Repository;
using DTO.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;

        Task SaveChangesAsync();

        void SaveChanges();
        Task RollbackAsync();

        IQueryable<T> ApplySort<T>(IQueryable<T> entities, IList<OrderingParameters> OrderingParameters) where T : class;
    }
}
