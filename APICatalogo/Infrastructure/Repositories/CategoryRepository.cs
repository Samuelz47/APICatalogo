using APICatalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public Category GetCategoryById(int id)
    {
        return _context.Categories.FirstOrDefault(c => c.Id == id);
    }

    public Category Create(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        _context.Categories.Add(category);
        _context.SaveChanges();
        return category;
    }

    public Category Update(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();
        return category;
    }

    public Category Delete(int id)
    {
        var category = _context.Categories.Find(id);

        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return category;
    }
}
