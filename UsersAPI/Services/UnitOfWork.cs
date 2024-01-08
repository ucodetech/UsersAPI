using AutoMapper;
using UsersAPI.Data;
using UsersAPI.Models;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserApiDbContext _db;
    public IProductService Product { get; private set; }
    public ICustomerService Customer { get; private set; }
    public IProductImageService ProductImage { get; private set; }
    
    public UnitOfWork(UserApiDbContext db)
    {
        _db = db;
        Product = new ProductService(_db);
        ProductImage = new ProductImageService(_db);
        Customer = new CustomerService(_db);
    }
    

    public void Save()
    {
        _db.SaveChanges();
    }
}