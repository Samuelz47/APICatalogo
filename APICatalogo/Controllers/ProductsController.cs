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
using X.PagedList;

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
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()                         //ActionResult funciona como um tipo de retorno pra aceitar o NotFound caso o retorno não seja um Enumerable<Product>
    {
        var products = await _uof.ProductRepository.GetAllAsync();
        if (products is null)
        {
            return NotFound("Produtos não encontrados");
        }
        //var destino = _mapper.map<Destino>(origem)
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
        return Ok(productsDto);
    }
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsPara)
    {
        var products = await _uof.ProductRepository.GetProductsAsync(productsPara);

        return GetProducts(products);
    }

    [HttpGet("filter/price/pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFilterPrice([FromQuery] FilterProductsPrice filterProductsPara)
    {
        var products = await _uof.ProductRepository.GetProductsFilterPriceAsync(filterProductsPara);

        return GetProducts(products);
    }

    [HttpGet("{id:int:min(1)}", Name ="GetProduct")]
    public async Task<ActionResult<ProductDTO>> Get(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(c => c.Id == id);
        if (product is null)
        {
            return NotFound("Id inexistente");
        }
        var productDto = _mapper.Map<ProductDTO>(product);
        return Ok(productDto);
    }

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByCategory (int id)
    {
        var products = await _uof.ProductRepository.GetProductsByCategoryAsync(id);
        if(products is null)
        {
            return NotFound("Produtos não encontrado");
        }
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
        return Ok(productsDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDto)
    {
        if (productDto is null)
        {
            return BadRequest();
        }
        var product = _mapper.Map<Product>(productDto);
        var newProduct = _uof.ProductRepository.Create(product);
        await _uof.CommitAsync();

        var newProductDto = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult("GetProduct", new { id = newProductDto.Id }, newProductDto);
        //Aciona a rota GetProduct com o ID do produto criado e retorna o produto com os dados amostra.
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDto)
    {
        if (patchProductDto is null)
        {
            return BadRequest("Produto vazio");
        }
        if (id <= 0)
        {
            return BadRequest("ID inválido");
        }

        var product = await _uof.ProductRepository.GetAsync(c => c.Id == id); //Localizando o produto
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
        await _uof.CommitAsync();

        return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
    }
    
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult<ProductDTO>> Put(int id, ProductDTO productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }
        var product = _mapper.Map<Product>(productDto);
        var updatedProduct = _uof.ProductRepository.Update(product);
        await _uof.CommitAsync();
        var updatedProductDto = _mapper.Map<ProductDTO>(updatedProduct);

        return Ok(updatedProductDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        var deletedProduct = await _uof.ProductRepository.GetAsync(c => c.Id == id);

        if (deletedProduct is null)
        {
            return NotFound("Produto não encontrado");
        }
        _uof.ProductRepository.Delete(deletedProduct);
        await _uof.CommitAsync();
        var deletedProductDto = _mapper.Map<ProductDTO>(deletedProduct);

        return Ok(deletedProductDto);
    }

    private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product> products)
    {
        var metadata = new
        {
            products.PageCount,
            products.TotalItemCount,
            products.PageSize,
            products.Count,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDto);
    }
}
