using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)             //Usa a injeção de dependencia aplicada na classe Repository
    {
    }

    public async Task<PagedList<Category>> GetCategoriesAsync(CategoryParameters categoryParameters)
    {
        var categories =  await GetAllAsync();
        var orderedCategories = categories.OrderBy(p => p.Id).AsQueryable();

        var result = PagedList<Category>.ToPagedList(orderedCategories, categoryParameters.PageNumber, categoryParameters.PageSize);
        return result;
    }

    public async Task<PagedList<Category>> GetCategoriesByNameAsync(FilterCategoryName filterCategoryName)
    {
        var categories = await GetAllAsync();

        if (!string.IsNullOrEmpty(filterCategoryName.Name))
        {
            categories = categories.Where(c => c.Name.Contains(filterCategoryName.Name));
        }

        var filterCategory = PagedList<Category>.ToPagedList(categories.AsQueryable(), filterCategoryName.PageNumber, filterCategoryName.PageSize);

        return filterCategory;
    }
}
