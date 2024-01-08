using Microsoft.EntityFrameworkCore;
using UsersAPI.Data;
using UsersAPI.Models;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public class ProductService : Repository<Product>, IProductService
{
    private readonly UserApiDbContext _db;
    public ProductService(UserApiDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task Update(Product product)
    {
        var productDb = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (productDb != null)
        {
            productDb.Title = product.Title;
            productDb.Price = product.Price;
            productDb.Description = product.Description;
            productDb.Stock = product.Stock;

        }
       
    }
}