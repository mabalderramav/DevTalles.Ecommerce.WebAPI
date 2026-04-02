using AutoMapper;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Models.Dtos;

namespace DevTalles.Ecommerce.WebAPI.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
    }
}