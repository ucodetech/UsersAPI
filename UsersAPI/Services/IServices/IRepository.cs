using System.Linq.Expressions;

namespace UsersAPI.Services.IServices;

public interface IRepository<T> where T: class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> Get(Expression<Func<T, bool>> filter);
    Task  Add(T entity);
    void Remove(T entity);
    void RemoveRange(T entities);
    Task<IEnumerable<T>> GetAllById(Expression<Func<T, bool>> filter);
}