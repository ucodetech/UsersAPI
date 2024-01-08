using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Models;

public class CustomerModal
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [DisplayName(displayName:"Full Name")]
    public string Fullname { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    [StringLength(11, MinimumLength = 11)]
    [DisplayName(displayName:"Phone Number")]
    public string PhoneNo { get; set; }
    [Required]
    public string Gender { get; set; }
    [StringLength(255, MinimumLength = 20)]
    public string HomeAddress { get; set; }
    
    [Required]
    [DisplayName(displayName:"Unique ID")]
    public Guid UniqueId { get; set; }
    
    public CustomerModal()
    {
        UniqueId = Guid.NewGuid();
    }
    
    public bool? IsActive { get; set; }
    
    public string? StatusName { get; set; }
}