using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public PagedList<Product> GetProducts(ProductsParameters productsParameters)
    {
        var products = GetAll().OrderBy(p => p.Id).AsQueryable();
        var orderedProducts = PagedList<Product>.ToPagedList(products, productsParameters.PageNumber, productsParameters.PageSize);
        return orderedProducts;
    }

    public IEnumerable<Product> GetProductsByCategory(int id)
    {
        return GetAll().Where(c => c.IdCategory == id);
    }

    public PagedList<Product> GetProductsFilterPrice(FilterProductsPrice filterProductsParameters)
    {
        var products = GetAll().AsQueryable();

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

        var filterProducts = PagedList<Product>.ToPagedList(products, filterProductsParameters.PageNumber, filterProductsParameters.PageSize);      //podemos paginar visto que a classe herda de QueryStrinParameters

        return filterProducts;
    }
}
