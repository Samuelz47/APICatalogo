using APICatalogo.Domain.Entities;
using AutoMapper;

namespace APICatalogo.DTOs.Mappings;

public class CategoryDTOMappingProfile : Profile
{
    public CategoryDTOMappingProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
    }
}
