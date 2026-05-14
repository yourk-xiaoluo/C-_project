using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WpfApp3
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// 用户数据存储管理类（基于本地文件）
    /// </summary>
    public static class UserStore
    {
        private static readonly string DataFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "users.dat");

        private static List<UserInfo> _users = new List<UserInfo>();

        static UserStore()
        {
            LoadUsers();
        }

        /// <summary>
        /// 加载用户数据
        /// </summary>
        private static void LoadUsers()
        {
            _users.Clear();
            if (!File.Exists(DataFilePath))
                return;

            try
            {
                string[] lines = File.ReadAllLines(DataFilePath, Encoding.UTF8);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        _users.Add(new UserInfo
                        {
                            Username = parts[0],
                            PasswordHash = parts[1],
                            Email = parts[2]
                        });
                    }
                }
            }
            catch (Exception)
            {
                // 文件读取失败，使用空列表
            }
        }

        /// <summary>
        /// 保存用户数据到文件
        /// </summary>
        private static void SaveUsers()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var user in _users)
                {
                    sb.AppendLine($"{user.Username}|{user.PasswordHash}|{user.Email}");
                }
                File.WriteAllText(DataFilePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception)
            {
                // 文件写入失败
            }
        }

        /// <summary>
        /// 验证用户登录凭据
        /// </summary>
        public static bool ValidateUser(string username, string password)
        {
            string hash = ComputeHash(password);
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return user != null && user.PasswordHash == hash;
        }

        /// <summary>
        /// 检查用户名是否已存在
        /// </summary>
        public static bool UserExists(string username)
        {
            return _users.Any(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 验证用户名与邮箱是否匹配
        /// </summary>
        public static bool ValidateUserEmail(string username, string email)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return user != null &&
                   user.Email.Equals(email, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        public static void RegisterUser(string username, string password, string email)
        {
            _users.Add(new UserInfo
            {
                Username = username,
                PasswordHash = ComputeHash(password),
                Email = email
            });
            SaveUsers();
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        public static void ResetPassword(string username, string newPassword)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.PasswordHash = ComputeHash(newPassword);
                SaveUsers();
            }
        }

        /// <summary>
        /// 计算密码哈希值（SHA256）
        /// </summary>
        private static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}