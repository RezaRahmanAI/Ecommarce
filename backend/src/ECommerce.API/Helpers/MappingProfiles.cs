using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;

namespace ECommerce.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.MediaUrls, o => o.MapFrom(s => s.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList()))
            .ForMember(d => d.Images, o => o.MapFrom(s => new ProductImagesDto
            {
                MainImage = new ImageDto 
                { 
                    Url = s.ImageUrl ?? "", 
                    Alt = s.Name,
                    Label = "Main"
                },
                Thumbnails = s.Images.Where(i => !i.IsMain).Select(i => new ImageDto
                {
                    Url = i.Url,
                    Alt = i.AltText ?? "",
                    Label = "Gallery"
                }).ToList()
            }));

        CreateMap<Category, CategoryDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
