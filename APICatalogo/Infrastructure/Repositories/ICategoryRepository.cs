using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using X.PagedList;

namespace APICatalogo.Infrastructure.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IPagedList<Category>> GetCategoriesAsync(CategoryParameters categoryParameters);
    Task<IPagedList<Category>> GetCategoriesByNameAsync(FilterCategoryName filterCategoryName);
}
