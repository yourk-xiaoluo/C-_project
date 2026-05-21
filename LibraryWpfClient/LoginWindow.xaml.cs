using System.Windows;
using LibraryWpfClient.Services;
using LibraryGrpcService;
using Grpc.Core;

namespace LibraryWpfClient
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名！");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("请输入密码！");
                return;
            }

            try
            {
                var client = GrpcClientService.CreateUserClient();
                var response = await client.ValidateUserAsync(new ValidateUserRequest
                {
                    Username = username,
                    Password = password
                });

                if (response.Success)
                {
                    Application.Current.Properties["CurrentUser"] = username;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
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
                ShowMessage($"登录失败：{ex.Message}");
            }
        }

        private void LinkRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        private void LinkResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ResetPasswordWindow resetWindow = new ResetPasswordWindow();
            resetWindow.Owner = this;
            resetWindow.ShowDialog();
        }

        private void ShowMessage(string message)
        {
            TxtMessage.Text = message;
            TxtMessage.Visibility = Visibility.Visible;
        }
    }
}