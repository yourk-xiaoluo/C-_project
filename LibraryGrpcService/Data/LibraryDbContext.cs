using LibraryGrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryGrpcService.Data;

/// <summary>
/// 图书管理系统数据库上下文
/// </summary>
public class LibraryDbContext : DbContext
{
    public DbSet<BookEntity> Books => Set<BookEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 种子数据 - 预置示例图书
        modelBuilder.Entity<BookEntity>().HasData(
            new BookEntity { Id = 1, Title = "红楼梦", Author = "曹雪芹", ISBN = "978-7-02-000220-0", Category = "文学小说" },
            new BookEntity { Id = 2, Title = "三体", Author = "刘慈欣", ISBN = "978-7-5366-9293-0", Category = "科学技术" },
            new BookEntity { Id = 3, Title = "百年孤独", Author = "加西亚·马尔克斯", ISBN = "978-7-5442-4528-0", Category = "文学小说" },
            new BookEntity { Id = 4, Title = "史记", Author = "司马迁", ISBN = "978-7-101-00304-1", Category = "历史传记" },
            new BookEntity { Id = 5, Title = "C# 本质论", Author = "Ben Albahari", ISBN = "978-7-115-41682-4", Category = "科学技术", IsBorrowed = true, Borrower = "张三", BorrowDate = DateTime.Now.AddDays(-3) },
            new BookEntity { Id = 6, Title = "人类简史", Author = "尤瓦尔·赫拉利", ISBN = "978-7-5086-4735-1", Category = "历史传记" },
            new BookEntity { Id = 7, Title = "论语", Author = "孔子", ISBN = "978-7-101-00352-6", Category = "哲学思想" },
            new BookEntity { Id = 8, Title = "艺术的故事", Author = "贡布里希", ISBN = "978-7-5495-0965-5", Category = "艺术设计" }
        );
    }
}