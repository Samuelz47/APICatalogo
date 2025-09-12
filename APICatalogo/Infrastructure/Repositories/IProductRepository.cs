using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using X.PagedList;

namespace APICatalogo.Infrastructure.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
    Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParameters);
    Task<IPagedList<Product>> GetProductsFilterPriceAsync(FilterProductsPrice filterProductsParameters);
}
