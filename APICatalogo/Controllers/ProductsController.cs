using APICatalogo.Domain.Entities;
using APICatalogo.DTOs;
using APICatalogo.Infrastructure;
using APICatalogo.Infrastructure.Repositories;
using APICatalogo.Shared.Pagination;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ProductsController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductDTO>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = _uof.ProductRepository.GetAll().ToList();                          
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        //var destino = _mapper.map<Destino>(origem)
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
        return Ok(productsDto);
    }
    [HttpGet("pagination")]
    public ActionResult<IEnumerable<ProductDTO>> Get([FromQuery] ProductsParameters productsPara)
    {
        var products = _uof.ProductRepository.GetProducts(productsPara);

        return GetProducts(products);
    }

    [HttpGet("filter/price/pagination")]
    public ActionResult<IEnumerable<ProductDTO>> GetProductsFilterPrice([FromQuery] FilterProductsPrice filterProductsPara)
    {
        var products = _uof.ProductRepository.GetProductsFilterPrice(filterProductsPara);

        return GetProducts(products);
    }

    [HttpGet("{id:int:min(1)}", Name ="GetProduct")]
    public ActionResult<ProductDTO> Get(int id)
    {
        var product = _uof.ProductRepository.Get(c => c.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }
        var productDto = _mapper.Map<ProductDTO>(product);
        return Ok(productDto);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<ProductDTO>> GetProductByCategory (int id)
    {
        var products = _uof.ProductRepository.GetProductsByCategory(id);
        if(products is null)
        {
            return NotFound("Produtos não encontrado");
        }
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
        return Ok(productsDto);
    }

    [HttpPost]
    public ActionResult<ProductDTO> Post(ProductDTO productDto)
    {
        if (productDto is null)
        {
            return BadRequest();
        }
        var product = _mapper.Map<Product>(productDto);
        var newProduct = _uof.ProductRepository.Create(product);
        _uof.Commit();

        var newProductDto = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult("GetProduct", new { id = newProductDto.Id }, newProductDto);
        //Aciona a rota GetProduct com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPatch("{id}/UpdatePartial")]
    public ActionResult<ProductDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDto)
    {
        if (patchProductDto is null)
        {
            return BadRequest("Produto vazio");
        }
        if (id <= 0)
        {
            return BadRequest("ID inválido");
        }

        var product = _uof.ProductRepository.Get(c => c.Id == id); //Localizando o produto
        if (product is null)
        {
            return NotFound("Produto não existe");
        }

        var productUpdateRequest = _mapper.Map<ProductDTOUpdateRequest>(product);
        patchProductDto.ApplyTo(productUpdateRequest, ModelState);      //ModelState serve para verificar qualquer erro de validação
        if(!ModelState.IsValid || !TryValidateModel(productUpdateRequest))
        {
            return BadRequest(ModelState);          //Caso o produto não esteja de acordo com as regras do DataAnnotations
        }

        _mapper.Map(productUpdateRequest, product);
        _uof.ProductRepository.Update(product);
        _uof.Commit();

        return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
    }
    
    [HttpPut("{id:int:min(1)}")]
    public ActionResult<ProductDTO> Put(int id, ProductDTO productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }
        var product = _mapper.Map<Product>(productDto);
        var updatedProduct = _uof.ProductRepository.Update(product);
        _uof.Commit();
        var updatedProductDto = _mapper.Map<ProductDTO>(updatedProduct);

        return Ok(updatedProductDto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProductDTO> Delete(int id)
    {
        var deletedProduct = _uof.ProductRepository.Get(c => c.Id == id);

        if (deletedProduct is null)
        {
            return NotFound("Produto não encontrado");
        }
        _uof.ProductRepository.Delete(deletedProduct);
        _uof.Commit();
        var deletedProductDto = _mapper.Map<ProductDTO>(deletedProduct);

        return Ok(deletedProductDto);
    }

    private ActionResult<IEnumerable<ProductDTO>> GetProducts(PagedList<Product> products)
    {
        var metadata = new
        {
            products.CurrentPage,
            products.TotalPages,
            products.PageSize,
            products.TotalCount,
            products.HasNexts,
            products.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDto);
    }
}
