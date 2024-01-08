using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 5)]
    [DisplayName(displayName:"User Name")]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }

 
}