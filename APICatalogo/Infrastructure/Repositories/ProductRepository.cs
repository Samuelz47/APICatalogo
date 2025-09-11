using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Product>> GetProductsAsync(ProductsParameters productsParameters)
    {
        var products = await GetAllAsync();
        var orderedProducts = products.OrderBy(p => p.Id).AsQueryable();
        var result = PagedList<Product>.ToPagedList(orderedProducts, productsParameters.PageNumber, productsParameters.PageSize);
        return result;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
    {
        var products = await GetAllAsync();
        return products.Where(c => c.IdCategory == id);
    }

    public async Task<PagedList<Product>> GetProductsFilterPriceAsync(FilterProductsPrice filterProductsParameters)
    {
        var products = await GetAllAsync();

        if (filterProductsParameters.Price.HasValue && !string.IsNullOrEmpty(filterProductsParameters.PriceCriterion))
        {
            if(filterProductsParameters.PriceCriterion.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price > filterProductsParameters.Price.Value).OrderBy(p => p.Price);
            }
            else if(filterProductsParameters.PriceCriterion.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price < filterProductsParameters.Price.Value).OrderBy(p => p.Price);
            }
            else if (filterProductsParameters.PriceCriterion.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                products = products.Where(p => p.Price == filterProductsParameters.Price.Value).OrderBy(p => p.Price);
            }
        }

        var filterProducts = PagedList<Product>.ToPagedList(products.AsQueryable(), filterProductsParameters.PageNumber, filterProductsParameters.PageSize);      //podemos paginar visto que a classe herda de QueryStrinParameters

        return filterProducts;
    }
}
