using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UsersAPI.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    [Required]
    [DataType(dataType:DataType.Currency)]
    public float Price { get; set; }
    [Required]
    [Range(1,1000)]
    public int Stock { get; set; }
    [Required]
    [StringLength(5)]
    public string ProductCode { get; set; }
    
    
}