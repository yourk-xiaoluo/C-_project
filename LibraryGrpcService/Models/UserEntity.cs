using System.ComponentModel.DataAnnotations;

namespace LibraryGrpcService.Models;

/// <summary>
/// 用户实体
/// </summary>
public class UserEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
}