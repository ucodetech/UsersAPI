using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Models;

public class ProductImage
{
    [Key]
    public int Id { get; set; }
    [Required]
    [DisplayName(displayName:"Product Code")]
    [StringLength(5)]
    public string ProductCode { get; set; }
    public string ProductFile { get; set; }
}