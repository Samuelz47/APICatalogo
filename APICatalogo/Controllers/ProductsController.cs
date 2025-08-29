using APICatalogo.Domain.Entities;
using APICatalogo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<Product>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = _context.Products.ToList();                          //Cria a lista de produtos, onde usamos Enumerable pois consome menos memoria nesse caso
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return products;
    }
}
