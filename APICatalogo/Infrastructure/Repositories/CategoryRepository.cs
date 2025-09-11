using APICatalogo.Domain.Entities;
using APICatalogo.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)             //Usa a injeção de dependencia aplicada na classe Repository
    {
    }

    public PagedList<Category> GetCategories(CategoryParameters categoryParameters)
    {
        var categories = GetAll().OrderBy(p => p.Id).AsQueryable();
        var orderedCategories = PagedList<Category>.ToPagedList(categories, categoryParameters.PageNumber, categoryParameters.PageSize);
        return orderedCategories;
    }

    public PagedList<Category> GetCategoriesByName(FilterCategoryName filterCategoryName)
    {
        var categories = GetAll().AsQueryable();

        if (!string.IsNullOrEmpty(filterCategoryName.Name))
        {
            categories = categories.Where(c => c.Name.Contains(filterCategoryName.Name));
        }

        var filterCategory = PagedList<Category>.ToPagedList(categories, filterCategoryName.PageNumber, filterCategoryName.PageSize);

        return filterCategory;
    }
}
