using APICatalogo.Domain.Entities;
using APICatalogo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = await _context.Products.AsNoTracking().ToListAsync();                          //Cria a lista de produtos, onde usamos Enumerable pois consome menos memoria nesse caso
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return products;
    }

    [HttpGet("{id:int:min(1)}", Name ="GetProduct")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }
        return product;
    }

    [HttpPost]
    public ActionResult Post(Product product)
    {
        if (product is null)
        {
            return BadRequest();
        }

        _context.Products.Add(product);
        _context.SaveChanges();

        return new CreatedAtRouteResult("GetProduct", new { id = product.Id }, product);
        //Aciona a rota GetProduct com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return Ok(product);
    }
}
