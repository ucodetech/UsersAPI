using Microsoft.AspNetCore.Mvc.ApiExplorer;
using UsersAPI.Models;
using UsersAPI.Services.IServices;

namespace UsersAPI.Services;

public interface ICustomerService : IRepository<Customer>
{
   void Update(Customer customer);

}