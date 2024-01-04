using AutoMapper;
using E_commerceAPI.Models;
using E_commerceAPI.Models.DTO;

namespace E_commerceAPI
{
    public class MappinConfig : Profile
    {
        public MappinConfig()
        {
            CreateMap<Contact, ContactRequestDTO>().ReverseMap();
            CreateMap<Contact, ContactUpdateDTO>().ReverseMap();
        }
    }
}
