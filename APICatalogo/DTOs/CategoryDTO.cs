using APICatalogo.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class CategoryDTO
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(80)]
    [Uppercase]
    public string? Name { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
}
