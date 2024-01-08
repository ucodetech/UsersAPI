using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Data;
using UsersAPI.Models;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public class ProductImageService : Repository<ProductImage>, IProductImageService
{
    private readonly UserApiDbContext _db;
    public ProductImageService(UserApiDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ProductImage productImage)
    {
        _db.ProductImages.Update(productImage);
     
    }
}