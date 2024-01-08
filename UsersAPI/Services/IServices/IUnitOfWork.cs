using System.Xml.Linq;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public interface IUnitOfWork
{
    IProductService Product { get; }
    ICustomerService Customer { get; }
    IProductImageService ProductImage { get; }
    void Save();
}