using APICatalogo.Domain.Entities;
using APICatalogo.Filters;
using APICatalogo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]       //Aplicando filtro de loginng
    public async Task<ActionResult<IEnumerable<Category>>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Category>
    {
        //Estamos usando async visto que o método faz consulta direta no Banco de Dados
        var categories =  await _context.Categories.AsNoTracking().ToListAsync();                          //Cria a lista de produtos, onde usamos Enumerable pois consome menos memoria nesse caso
        if (categories is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return categories;
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<Category>> Get(int id)
    {
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);      //AsNoTracking é usado pras consultas nao serem rastreadas assim diminuindo sobrecarga na memoria
        if (category is null)
        {
            return NotFound("Id inexistente");
        }
        return category;
    }

    [HttpGet("products")]                   //Não podemos ter duas rotas iguais, por isso dado o nome a essa
    public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesProducts()
    {
        return await _context.Categories.Include(p => p.Products).AsNoTracking().ToListAsync();
    }

    [HttpPost]
    public ActionResult Post(Category category)
    {
        if (category is null)
        {
            return BadRequest();
        }

        _context.Categories.Add(category);
        _context.SaveChanges();

        return new CreatedAtRouteResult("GetCategory", new { id = category.Id }, category);
        //Aciona a rota GetCategory com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var category = _context.Categories.FirstOrDefault(p => p.Id == id);
        if (category is null)
        {
            return NotFound("Id inexistente");
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();

        return Ok(category);
    }
}
