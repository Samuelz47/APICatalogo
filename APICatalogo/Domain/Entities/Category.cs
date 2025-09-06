﻿using APICatalogo.Domain.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APICatalogo.Domain.Entities;

public class Category
{
    public Category()
    {
        Products = new Collection<Product>();
    }
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(80)]
    [Uppercase]
    public string? Name { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
    [JsonIgnore]
    public ICollection<Product>? Products { get; set; }
}
