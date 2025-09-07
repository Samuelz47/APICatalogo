using APICatalogo.Domain.Entities;
using APICatalogo.Filters;
using APICatalogo.Infrastructure;
using APICatalogo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    public CategoryController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]       //Aplicando filtro de loginng
    public ActionResult<IEnumerable<Category>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Category>
    {
        var categories = _uof.CategoryRepository.GetAll();
        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public ActionResult<Category> Get(int id)
    {
        var category = _uof.CategoryRepository.Get(c => c.Id == id);      
        if (category is null)
        {
            return NotFound("Id inexistente");
        }
        return Ok(category);
    }

    [HttpPost]
    public ActionResult Post(Category category)
    {
        if (category is null)
        {
            return BadRequest();
        }

        var createdcategory = _uof.CategoryRepository.Create(category);
        _uof.Commit();

        return new CreatedAtRouteResult("GetCategory", new { id = createdcategory.Id }, category);
        //Aciona a rota GetCategory com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult Put(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        _uof.CategoryRepository.Update(category);
        _uof.Commit();

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var category = _uof.CategoryRepository.Get(c => c.Id == id);
        if (category is null)
        {
            return NotFound("Id inexistente");
        }

        var excludedCategory = _uof.CategoryRepository.Delete(category);
        _uof.Commit();

        return Ok(excludedCategory);
    }
}
