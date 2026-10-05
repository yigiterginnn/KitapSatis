using System.Linq.Expressions;
using KitapSatis.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace KitapSatis.Web.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<List<T>> WhereAsync(Expression<Func<T, bool>> filter)
        => await _dbSet.Where(filter).ToListAsync();

    public IQueryable<T> Query()
        => _dbSet.AsQueryable();

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();
}