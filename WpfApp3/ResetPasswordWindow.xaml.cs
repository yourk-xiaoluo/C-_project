using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfApp3
{
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重置密码按钮点击事件
        /// </summary>
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string email = TxtEmail.Text.Trim();
            string newPassword = TxtNewPassword.Password;
            string confirmPassword = TxtConfirmPassword.Password;

            // 验证输入
            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名！", false);
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("请输入注册邮箱！", false);
                return;
            }
            if (!IsValidEmail(email))
            {
                ShowMessage("请输入有效的邮箱地址！", false);
                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                ShowMessage("请输入新密码！", false);
                return;
            }
            if (newPassword.Length < 6)
            {
                ShowMessage("新密码长度不能少于6个字符！", false);
                return;
            }
            if (newPassword != confirmPassword)
            {
                ShowMessage("两次输入的密码不一致！", false);
                return;
            }

            // 验证用户名和邮箱是否匹配
            if (!UserStore.UserExists(username))
            {
                ShowMessage("该用户名不存在！", false);
                return;
            }

            if (!UserStore.ValidateUserEmail(username, email))
            {
                ShowMessage("用户名与邮箱不匹配！", false);
                return;
            }

            // 重置密码
            UserStore.ResetPassword(username, newPassword);
            ShowMessage("密码重置成功！请返回登录。", true);
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