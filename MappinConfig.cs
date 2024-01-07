using AutoMapper;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;

namespace E_commerceAPI
{
    public class MappinConfig : Profile
    {
        public MappinConfig()
        {
            CreateMap<Contact, ContactCreateDTO>().ReverseMap();
            CreateMap<Contact, ContactUpdateDTO>().ReverseMap();
            CreateMap<Product, ProductCreateDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();
        }
    }
}
