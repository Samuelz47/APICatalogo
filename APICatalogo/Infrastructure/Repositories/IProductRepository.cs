using APICatalogo.Domain.Entities;

namespace APICatalogo.Infrastructure.Repositories;

public interface IProductRepository : IRepository<Product>
{
    IEnumerable<Product> GetProductsByCategory(int id);
}
