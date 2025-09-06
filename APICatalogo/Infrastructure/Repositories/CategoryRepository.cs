using APICatalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)             //Usa a injeção de dependencia aplicada na classe Repository
    {
    }
}
