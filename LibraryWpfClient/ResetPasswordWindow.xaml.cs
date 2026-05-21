using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using LibraryWpfClient.Services;
using LibraryGrpcService;
using Grpc.Core;

namespace LibraryWpfClient
{
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        private async void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string newPassword = TxtNewPassword.Password;
            string confirmPassword = TxtConfirmPassword.Password;

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名！");
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("请输入注册邮箱！");
                return;
            }
            if (!IsValidEmail(email))
            {
                ShowMessage("请输入有效的邮箱地址！");
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                ShowMessage("请输入新密码！");
                return;
            }
            if (newPassword.Length < 6)
            {
                ShowMessage("密码长度不能少于6个字符！");
                return;
            }
            if (newPassword != confirmPassword)
            {
                ShowMessage("两次输入的密码不一致！");
                return;
            }

            try
            {
                var client = GrpcClientService.CreateUserClient();

                // 先验证用户名与邮箱是否匹配
                var validateResponse = await client.ValidateUserEmailAsync(new ValidateUserEmailRequest
                {
                    Username = username,
                    Email = email
                });

                if (!validateResponse.Success)
                {
                    ShowMessage("用户名与邮箱不匹配！");
                    return;
                }

                // 重置密码
                var response = await client.ResetPasswordAsync(new ResetPasswordRequest
                {
                    Username = username,
                    Email = email,
                    NewPassword = newPassword
                });

                if (response.Success)
                {
                    ShowSuccess(response.Message);
                    // 3秒后关闭
                    await Task.Delay(2000);
                    this.Close();
                }
                else
                {
                    ShowMessage(response.Message);
                }
            }
            catch (RpcException ex)
            {
                ShowMessage($"连接服务器失败：{ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                ShowMessage($"重置失败：{ex.Message}");
            }
        }

        private void LinkBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private void ShowMessage(string message)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = new SolidColorBrush(Colors.Red);
            TxtMessage.Visibility = Visibility.Visible;
        }

        private void ShowSuccess(string message)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            TxtMessage.Visibility = Visibility.Visible;
        }
    }
}