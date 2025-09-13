using APICatalogo.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Infrastructure;

public class AppDbContext : IdentityDbContext                       //Classe que irá fazer o mapeamento das tabelas pro banco de dados
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }             //Propriedades que mapeiam a tabela pra ser criada no banco de dados
    public DbSet<Product> Products { get; set; }
}
