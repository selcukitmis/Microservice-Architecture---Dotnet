using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T item);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(T item);
    }
}