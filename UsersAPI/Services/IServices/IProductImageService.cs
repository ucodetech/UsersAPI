using UsersAPI.Models;

namespace UsersAPI.Services.IServices;

public interface IProductImageService : IRepository<ProductImage>
{
    void Update(ProductImage productImage);
}