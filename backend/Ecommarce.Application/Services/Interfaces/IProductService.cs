namespace Ecommarce.Application.Services.Interfaces;

using Ecommarce.Application.Features.Products.DTOs;
using Ecommarce.Application.Features.Products.Queries.GetProducts; // Retain query DTO usage for parameters or redefine params
using Ecommarce.Application.Features.Products.Commands.CreateProduct; // Retain command DTO usage

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(GetProductsQuery query);
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductCommand command);
    Task<bool> DeleteProductAsync(int id);
}
