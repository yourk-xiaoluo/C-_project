using LibraryGrpcService.Data;
using LibraryGrpcService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加 gRPC 服务
builder.Services.AddGrpc();

// 配置 EF Core（SQLite）
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 确保数据库已创建（包含种子数据）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.EnsureCreated();
}

// 配置 gRPC 服务端点
app.MapGrpcService<BookGrpcService>();
app.MapGrpcService<UserGrpcService>();

app.MapGet("/", () => "图书管理系统 gRPC 服务已启动。请使用 gRPC 客户端进行通信。");

app.Run();