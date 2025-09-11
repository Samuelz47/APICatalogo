using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;

namespace APICatalogo.Infrastructure.Repositories;

public interface IProductRepository : IRepository<Product>
{
    IEnumerable<Product> GetProductsByCategory(int id);
    PagedList<Product> GetProducts(ProductsParameters productsParameters);
    PagedList<Product> GetProductsFilterPrice(FilterProductsPrice filterProductsParameters);
}
