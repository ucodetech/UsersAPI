using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using UsersAPI.Data;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly UserApiDbContext _db;
    private DbSet<T> _dbSet;

    public Repository(UserApiDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAll()
    {
        IQueryable<T> query = _dbSet;
        return await query.ToListAsync();
    }

    public async Task<T> Get(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;
        query = query.Where(filter);
        return await query.FirstOrDefaultAsync();
    }

    public async Task Add(T entity)
    {
       await _dbSet.AddAsync(entity);
       
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
        
    }

    public void RemoveRange(T entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<IEnumerable<T>> GetAllById(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;
        query = query.Where(filter);
        return await query.ToListAsync();
    }
}