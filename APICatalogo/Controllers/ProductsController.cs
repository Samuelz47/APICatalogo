using APICatalogo.Domain.Entities;
using APICatalogo.Infrastructure;
using APICatalogo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ProductsController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = _uof.ProductRepository.GetAll().ToList();                          
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return Ok(products);
    }

    [HttpGet("{id:int:min(1)}", Name ="GetProduct")]
    public ActionResult<Product> Get(int id)
    {
        var product = _uof.ProductRepository.Get(c => c.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }
        return Ok(product);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult GetProductByCategory (int id)
    {
        var products = _uof.ProductRepository.GetProductsByCategory(id);
        if(products is null)
        {
            return NotFound("Produtos não encontrado");
        }
        return Ok(products);
    }

    [HttpPost]
    public ActionResult Post(Product product)
    {
        if (product is null)
        {
            return BadRequest();
        }

        var newProduct = _uof.ProductRepository.Create(product);
        _uof.Commit();

        return new CreatedAtRouteResult("GetProduct", new { id = newProduct.Id }, newProduct);
        //Aciona a rota GetProduct com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        var updatedProduct = _uof.ProductRepository.Update(product);
        _uof.Commit();

        return Ok(updatedProduct);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deletedProduct = _uof.ProductRepository.Get(c => c.Id == id);

        if (deletedProduct is null)
        {
            return NotFound("Produto não encontrado");
        }
        _uof.ProductRepository.Delete(deletedProduct);
        _uof.Commit();

        return Ok(deletedProduct);
    }
}
