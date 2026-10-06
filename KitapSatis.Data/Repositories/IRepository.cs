using System.Linq.Expressions;
using KitapSatis.Data.Entities;

namespace KitapSatis.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> WhereAsync(Expression<Func<T, bool>> filter);
    IQueryable<T> Query();

    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();
}