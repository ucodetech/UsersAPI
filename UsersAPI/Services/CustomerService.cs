using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Data;
using UsersAPI.Models;

namespace UsersAPI.Services;

public class CustomerService : Repository<Customer>, ICustomerService
{
    private readonly UserApiDbContext _db;
    public CustomerService(UserApiDbContext db) : base(db)
    {
        _db = db;
    }
    

    public void Update(Customer customer)
    {
        var userObj =  _db.Customers.FirstOrDefault(c=>c.Id==customer.Id);
        if (userObj != null)
        {
            userObj.Fullname = customer.Fullname;
            userObj.Email = customer.Email;
            userObj.PhoneNo = customer.PhoneNo;
            userObj.Gender = customer.Gender;
           
        }
       
    }
    
}