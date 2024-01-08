using UsersAPI.Models;

namespace UsersAPI.Services.IServices;

public interface IProductService : IRepository<Product>
{
    Task Update(Product product);
}