using APICatalogo.Domain.Entities;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Infrastructure;
using APICatalogo.Infrastructure.Repositories;
using APICatalogo.Shared.Pagination;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    public CategoryController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]       //Aplicando filtro de loginng
    public ActionResult<IEnumerable<CategoryDTO>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Category>
    {
        var categories = _uof.CategoryRepository.GetAll().ToList();

        if (categories is null)
        {
            return NotFound("Categorias não encontrados");
        }
        
        var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        return Ok(categoriesDto);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public ActionResult<CategoryDTO> Get(int id)
    {
        var category = _uof.CategoryRepository.Get(c => c.Id == id);      
        if (category is null)
        {
            return NotFound("Id inexistente");
        }
        var categoryDto = _mapper.Map<CategoryDTO>(category);
        return Ok(category);
    }
    [HttpGet("pagination")]
    public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoryParameters categoriesPara)
    {
        var categories = _uof.CategoryRepository.GetCategories(categoriesPara);

        return GetCategories(categories);
    }

    [HttpGet("filter/name/pagination")]
    public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] FilterCategoryName filterCatName)
    {
        var categories = _uof.CategoryRepository.GetCategoriesByName(filterCatName);

        return GetCategories(categories);
    }

    [HttpPost]
    public ActionResult<CategoryDTO> Post(CategoryDTO categoryDto)
    {
        if (categoryDto is null)
        {
            return BadRequest();
        }

        var category = _mapper.Map<Category>(categoryDto);
        var createdCategory = _uof.CategoryRepository.Create(category);
        _uof.Commit();
        var createdCategoryDto = _mapper.Map<CategoryDTO>(createdCategory);

        return new CreatedAtRouteResult("GetCategory", new { id = createdCategoryDto.Id }, createdCategoryDto);
        //Aciona a rota GetCategory com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPut("{id:int:min(1)}")]
    public ActionResult<CategoryDTO> Put(int id, CategoryDTO categoryDto)
    {
        if (id != categoryDto.Id)
        {
            return BadRequest();
        }
        var category = _mapper.Map<Category>(categoryDto);
        var updatedCategory = _uof.CategoryRepository.Update(category);
        _uof.Commit();
        var updatedCategoryDto = _mapper.Map<CategoryDTO>(updatedCategory);

        return Ok(updatedCategoryDto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<CategoryDTO> Delete(int id)
    {
        var category = _uof.CategoryRepository.Get(c => c.Id == id);
        if (category is null)
        {
            return NotFound("Id inexistente");
        }

        var excludedCategory = _uof.CategoryRepository.Delete(category);
        _uof.Commit();
        var excludedCategoryDto = _mapper.Map<CategoryDTO>(excludedCategory);
        return Ok(excludedCategoryDto);
    }

    private ActionResult<IEnumerable<CategoryDTO>> GetCategories(PagedList<Category> categories)
    {
        var metadata = new
        {
            categories.CurrentPage,
            categories.TotalPages,
            categories.PageSize,
            categories.TotalCount,
            categories.HasNexts,
            categories.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

        return Ok(categoriesDto);
    }
}
