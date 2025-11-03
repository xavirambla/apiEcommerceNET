// Mapeos migrados a Mapster. Archivo dejado vacío para referencia..
/*
using Mapster;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models;

public static class ProductMappingConfig
{
    public static void RegisterProductMappings()
    {
        TypeAdapterConfig<Product, ProductDto>.NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);
        TypeAdapterConfig<CreateProductDto, Product>.NewConfig();
        TypeAdapterConfig<UpdateProductDto, Product>.NewConfig();
    }
}
*/