using APICatalogo.Domain.Entities;
using AutoMapper;

namespace APICatalogo.DTOs.Mappings;

public class ProductDTOMappingProfile : Profile
{
    public ProductDTOMappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();           //mapeamento de produto para produtodto e o reverso
        CreateMap<Product, ProductDTOUpdateRequest>().ReverseMap();
        CreateMap<Product, ProductDTOUpdateResponse>().ReverseMap();
    }
}
