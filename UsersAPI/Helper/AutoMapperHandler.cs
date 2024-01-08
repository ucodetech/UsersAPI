using AutoMapper;
using UsersAPI.Models;

namespace UsersAPI.Helper;

public class AutoMapperHandler : Profile
{
    public AutoMapperHandler()
    {
        CreateMap<Customer, CustomerModal>().ForMember(dest => dest.StatusName,
            opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "In active")).ReverseMap();
    }
}