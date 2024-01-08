using Microsoft.EntityFrameworkCore;
using UsersAPI.Models;

namespace UsersAPI.Data;

public class UserApiDbContext : DbContext
{
    public UserApiDbContext(DbContextOptions<UserApiDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Product> Products { get; set; }
}