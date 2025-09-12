using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace APICatalogo.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)             //Usa a injeção de dependencia aplicada na classe Repository
    {
    }

    public async Task<IPagedList<Category>> GetCategoriesAsync(CategoryParameters categoryParameters)
    {
        var categories =  await GetAllAsync();
        var orderedCategories = categories.OrderBy(p => p.Id).AsQueryable();

        var result = orderedCategories.ToPagedList(categoryParameters.PageNumber, categoryParameters.PageSize);
        return await Task.FromResult(result);
    }

    public async Task<IPagedList<Category>> GetCategoriesByNameAsync(FilterCategoryName filterCategoryName)
    {
        var categories = await GetAllAsync();

        if (!string.IsNullOrEmpty(filterCategoryName.Name))
        {
            categories = categories.Where(c => c.Name.Contains(filterCategoryName.Name, StringComparison.OrdinalIgnoreCase));
        }

        var filterCategory = categories.ToPagedList(filterCategoryName.PageNumber, filterCategoryName.PageSize);

        return await Task.FromResult(filterCategory);
    }
}
