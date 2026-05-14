using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp3
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadRememberedUser();
        }

        /// <summary>
        /// 加载记住的用户名和密码
        /// </summary>
        private void LoadRememberedUser()
        {
            string savedUser = Properties.Settings.Default.RememberedUser ?? "";
            string savedPass = Properties.Settings.Default.RememberedPass ?? "";
            if (!string.IsNullOrEmpty(savedUser))
            {
                TxtUsername.Text = savedUser;
                TxtPassword.Password = savedPass;
                ChkRemember.IsChecked = true;
            }
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;

            // 验证输入
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

            // 验证用户凭据
            if (UserStore.ValidateUser(username, password))
            {
                // 记住密码处理
                if (ChkRemember.IsChecked == true)
                {
                    Properties.Settings.Default.RememberedUser = username;
                    Properties.Settings.Default.RememberedPass = password;
                }
                else
                {
                    Properties.Settings.Default.RememberedUser = "";
                    Properties.Settings.Default.RememberedPass = "";
                }
                Properties.Settings.Default.Save();

                // 登录成功，打开主窗口
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ShowMessage("用户名或密码错误！");
            }
        }

        /// <summary>
        /// 注册链接点击事件
        /// </summary>
        private void LinkRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }

        /// <summary>
        /// 忘记密码链接点击事件
        /// </summary>
        private void LinkResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ResetPasswordWindow resetWindow = new ResetPasswordWindow();
            resetWindow.Owner = this;
            resetWindow.ShowDialog();
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        private void ShowMessage(string message)
        {
            TxtMessage.Text = message;
            TxtMessage.Visibility = Visibility.Visible;
        }
    }
}