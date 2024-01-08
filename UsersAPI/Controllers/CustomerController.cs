using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Identity.Client;
using UsersAPI.Models;
using UsersAPI.Services;

namespace UsersAPI.Controllers;

[Authorize]
[EnableRateLimiting("WindowFixedLimiter")]
[Route("api/[controller]")]
[ApiController]
public class CustomerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;

    }
    // GET

    [HttpGet("GetAll")]
    // public async Task<ActionResult<List<Customer>>> GetUsers()
    // {
    //     List<CustomerModal> response = new List<CustomerModal>();
    //     if ( await _unitOfWork.Customer.GetAll() is { } data)
    //     {
    //         response = _mapper.Map<List<Customer>, List<CustomerModal>>(data);
    //     }
    //     var customers =  _unitOfWork.Customer.GetAll();
    //     return customers;
    // }

  
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerModal>> GetByUniqueId(int id)
    {
        CustomerModal response = new CustomerModal();
        if (await _unitOfWork.Customer.Get(u=>u.Id==id) is { } data)
        {
            response = _mapper.Map<Customer, CustomerModal>(data);
        }
        return response;
   
    }
    
  
    [HttpPost("Create")]
    public async Task<ActionResult<ApiResponse<Customer>>> AddUser(CustomerModal user)
    {
        var response = new ApiResponse<string>();
        try
        {
            var cusObj = await _unitOfWork.Customer.Get(u => u.Email == user.Email);
            if (cusObj != null)
            {
                response.StatusCode = 300;
                response.Message = "Email is already taken!";
            }
            if (ModelState.IsValid)
            {
                Customer customer = _mapper.Map<CustomerModal, Customer>(user);
                _unitOfWork.Customer.Add(customer);
                _unitOfWork.Save();
                response.StatusCode = 200;
                response.Message = "User Added successfully";
                response.Data = user.Fullname;
            }
            else
            {
                response.StatusCode = 300;
                response.Message = "All fields are required!";
            }
        
        }   
        catch (Exception e)
        {
            response.StatusCode = 404;
            response.Message = e.Message;
        
        }
        
        return  Ok(response);
        
    }
    
  
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<Customer>>> RemoveUser(int id)
    {
        ApiResponse<string> response = new ApiResponse<string>();
        try
        {
            var userObj = await _unitOfWork.Customer.Get(u=>u.Id==id);
            if (userObj != null)
            {
                _unitOfWork.Customer.Remove(userObj);
                 _unitOfWork.Save();
                response.StatusCode = 200;
                response.Message = "Customer Removed!";
            }
            else
            {
                response.StatusCode = 404;
                response.Message = "User Not Found!";
            }
            
        }
        catch (Exception e)
        {
            response.StatusCode = 404;
            response.Message = e.Message;
        }
        
        return Ok(response);
       
    }

 
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Customer>>> UpdateUser(Customer request, int id)
    {
        ApiResponse<Customer> response = new ApiResponse<Customer>();
        try
        {
            var userObj = await _unitOfWork.Customer.Get(u=>u.Id==id);
            if (userObj != null)
            {
                userObj.Fullname = request.Fullname;
                userObj.Email = request.Email;
                userObj.PhoneNo = request.PhoneNo;
                userObj.Gender = request.Gender;
                 _unitOfWork.Save();
               
                response.StatusCode = 200;
                response.Message = "Customer updated!";
                response.Data = userObj;
            }
            else
            {
                response.StatusCode = 200;
                response.Message = "Customer not Found";
            }
        
           
        }
        catch (Exception e)
        {
            response.StatusCode = 404;
            response.Message = e.Message;
        }
        return Ok(response);
    }
}