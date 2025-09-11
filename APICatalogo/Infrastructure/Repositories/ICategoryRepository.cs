using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;

namespace APICatalogo.Infrastructure.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<PagedList<Category>> GetCategoriesAsync(CategoryParameters categoryParameters);
    Task<PagedList<Category>> GetCategoriesByNameAsync(FilterCategoryName filterCategoryName);
}
