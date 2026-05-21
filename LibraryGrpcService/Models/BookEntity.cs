using System.ComponentModel.DataAnnotations;

namespace LibraryGrpcService.Models;

/// <summary>
/// 图书实体
/// </summary>
public class BookEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    public bool IsBorrowed { get; set; }

    [MaxLength(100)]
    public string? Borrower { get; set; }

    public DateTime? BorrowDate { get; set; }
}