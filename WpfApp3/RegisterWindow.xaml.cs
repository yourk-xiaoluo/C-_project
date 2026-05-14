using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfApp3
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 注册按钮点击事件
        /// </summary>
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;
            string confirmPassword = TxtConfirmPassword.Password;
            string email = TxtEmail.Text.Trim();

            // 验证输入
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

            // 尝试注册
            if (UserStore.UserExists(username))
            {
                ShowMessage("该用户名已被注册！", false);
                return;
            }

            // 注册成功
            UserStore.RegisterUser(username, password, email);
            ShowMessage("注册成功！请返回登录。", true);
        }

        /// <summary>
        /// 返回登录链接点击事件
        /// </summary>
        private void LinkBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        private void ShowMessage(string message, bool isSuccess)
        {
            TxtMessage.Text = message;
            TxtMessage.Foreground = isSuccess
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 175, 80))
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            TxtMessage.Visibility = Visibility.Visible;
        }
    }
}