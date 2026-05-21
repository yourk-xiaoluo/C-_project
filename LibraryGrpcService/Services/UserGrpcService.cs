using Grpc.Core;
using LibraryGrpcService.Data;
using LibraryGrpcService.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LibraryGrpcService.Services;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly LibraryDbContext _db;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(LibraryDbContext db, ILogger<UserGrpcService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<ValidateUserResponse> ValidateUser(ValidateUserRequest request, ServerCallContext context)
    {
        var hash = ComputeHash(request.Password);
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower(),
            context.CancellationToken);

        if (user != null && user.PasswordHash == hash)
        {
            _logger.LogInformation("用户登录成功: {Username}", request.Username);
            return new ValidateUserResponse { Success = true, Message = "登录成功" };
        }

        return new ValidateUserResponse { Success = false, Message = "用户名或密码错误" };
    }

    public override async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
    {
        // 检查用户名是否已存在
        var exists = await _db.Users.AnyAsync(
            u => u.Username.ToLower() == request.Username.ToLower(),
            context.CancellationToken);

        if (exists)
        {
            return new RegisterUserResponse { Success = false, Message = "用户名已存在" };
        }

        var entity = new UserEntity
        {
            Username = request.Username,
            PasswordHash = ComputeHash(request.Password),
            Email = request.Email
        };

        _db.Users.Add(entity);
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("新用户注册: {Username}", request.Username);
        return new RegisterUserResponse { Success = true, Message = "注册成功" };
    }

    public override async Task<UserExistsResponse> UserExists(UserExistsRequest request, ServerCallContext context)
    {
        var exists = await _db.Users.AnyAsync(
            u => u.Username.ToLower() == request.Username.ToLower(),
            context.CancellationToken);

        return new UserExistsResponse { Exists = exists };
    }

    public override async Task<ValidateUserEmailResponse> ValidateUserEmail(ValidateUserEmailRequest request, ServerCallContext context)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower(),
            context.CancellationToken);

        var success = user != null &&
                      user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase);

        return new ValidateUserEmailResponse { Success = success };
    }

    public override async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username.ToLower() == request.Username.ToLower(),
            context.CancellationToken);

        if (user == null)
        {
            return new ResetPasswordResponse { Success = false, Message = "用户不存在" };
        }

        if (!user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            return new ResetPasswordResponse { Success = false, Message = "邮箱不匹配" };
        }

        user.PasswordHash = ComputeHash(request.NewPassword);
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("用户重置密码: {Username}", request.Username);
        return new ResetPasswordResponse { Success = true, Message = "密码重置成功" };
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}