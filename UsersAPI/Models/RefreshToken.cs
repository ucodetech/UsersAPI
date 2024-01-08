using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UsersAPI.Models;

public class RefreshToken
{
    [Key]
    public string userId { get; set; }
    [StringLength(50)]
    public string tokenId { get; set; }
    public string refreshToken { get; set; }
}