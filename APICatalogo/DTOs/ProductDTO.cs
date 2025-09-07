using APICatalogo.Domain.Entities;
using APICatalogo.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    [Required]
    [StringLength(80)]
    [Uppercase]
    public string? Name { get; set; }
    [Required]
    [StringLength(300)]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
    public int IdCategory { get; set; }
}
