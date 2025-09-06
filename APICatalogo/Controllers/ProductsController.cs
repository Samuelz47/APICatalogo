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
    private readonly IProductRepository _productRepository;         //como temos funções especificas pra esse repositorio é necessario a injeção de dependencia dele, alem do generico
    private readonly IRepository<Product> _repository;

    public ProductsController(IProductRepository productRepository, IRepository<Product> repository)
    {
        _productRepository = productRepository;
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = _repository.GetAll().ToList();                          
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        return Ok(products);
    }

    [HttpGet("{id:int:min(1)}", Name ="GetProduct")]
    public ActionResult<Product> Get(int id)
    {
        var product = _repository.Get(c => c.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }
        return Ok(product);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult GetProductByCategory (int id)
    {
        var products = _productRepository.GetProductsByCategory(id);
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

        var newProduct = _repository.Create(product);

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

        var updatedProduct = _repository.Update(product);
        
        return Ok(updatedProduct);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deletedProduct = _repository.Get(c => c.Id == id);

        if (deletedProduct is null)
        {
            return NotFound("Produto não encontrado");
        }
        _repository.Delete(deletedProduct);
        return Ok(deletedProduct);
    }
}
