using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;

namespace APICatalogo.Infrastructure.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
    Task<PagedList<Product>> GetProductsAsync(ProductsParameters productsParameters);
    Task<PagedList<Product>> GetProductsFilterPriceAsync(FilterProductsPrice filterProductsParameters);
}
