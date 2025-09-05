using APICatalogo.Domain.Entities;

namespace APICatalogo.Infrastructure.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();
    Category GetCategoryById(int id);
    Category Create(Category category);
    Category Update(Category category);
    Category Delete(int id);
}
