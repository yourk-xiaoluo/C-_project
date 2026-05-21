using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using LibraryWpfClient.Services;
using LibraryGrpcService;
using Grpc.Core;

namespace LibraryWpfClient
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;
            string confirmPassword = TxtConfirmPassword.Password;
            string email = TxtEmail.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名！", false);
                return;
            }
            if (username.Length < 3 || username.Length > 20)
            {
                ShowMessage("用户名长度应为3-20个字符！", false);
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("请输入密码！", false);
                return;
            }
            if (password.Length < 6)
            {
                ShowMessage("密码长度不能少于6个字符！", false);
                return;
            }
            if (password != confirmPassword)
            {
                ShowMessage("两次输入的密码不一致！", false);
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("请输入邮箱地址！", false);
                return;
            }
            if (!IsValidEmail(email))
            {
                ShowMessage("请输入有效的邮箱地址！", false);
                return;
            }

            try
            {
                var client = GrpcClientService.CreateUserClient();
                var response = await client.RegisterUserAsync(new RegisterUserRequest
                {
                    Username = username,
                    Password = password,
                    Email = email
                });

                ShowMessage(response.Message, response.Success);
            }
            catch (RpcException ex)
            {
                ShowMessage($"连接服务器失败：{ex.Status.Detail}", false);
            }
            catch (Exception ex)
            {
                ShowMessage($"注册失败：{ex.Message}", false);
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

        private void ShowMessage(string message, bool isSuccess)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = isSuccess
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Colors.Red);
            TxtMessage.Visibility = Visibility.Visible;
        }
    }
}