using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;

namespace APICatalogo.Infrastructure.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    PagedList<Category> GetCategories(CategoryParameters categoryParameters);
    PagedList<Category> GetCategoriesByName(FilterCategoryName filterCategoryName);
}
