using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private Book _selectedBook;
        private ObservableCollection<Book> _currentBooks;

        public MainWindow()
        {
            InitializeComponent();

            // 设置欢迎信息
            string username = Application.Current.Properties["CurrentUser"] as string ?? "用户";
            TxtWelcome.Text = $"欢迎，{username}";

            // 初始化分类下拉框
            CmbCategory.ItemsSource = BookManager.Instance.Categories;

            // 初始化录入弹窗中的分类下拉框（排除"全部"）
            CmbNewCategory.ItemsSource = BookManager.Instance.Categories.Where(c => c != "全部").ToArray();
            CmbNewCategory.SelectedIndex = 0;

            // 初始显示全部图书
            CmbCategory.SelectedIndex = 0;
            RefreshBookList();
        }

        /// <summary>
        /// 刷新图书列表
        /// </summary>
        private void RefreshBookList()
        {
            string category = CmbCategory.SelectedItem as string ?? "全部";
            _currentBooks = BookManager.Instance.GetBooksByCategory(category);
            BookPanel.Children.Clear();

            foreach (var book in _currentBooks)
            {
                BookPanel.Children.Add(CreateBookCard(book));
            }

            TxtBookCount.Text = $"共 {_currentBooks.Count} 本";
        }

        /// <summary>
        /// 创建图书卡片
        /// </summary>
        private Border CreateBookCard(Book book)
        {
            bool isBorrowed = book.IsBorrowed;

            var card = new Border
            {
                Width = 210,
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(14),
                Margin = new Thickness(6),
                BorderThickness = new Thickness(1),
                BorderBrush = isBorrowed ? new SolidColorBrush(Color.FromRgb(224, 224, 224)) : new SolidColorBrush(Color.FromRgb(180, 210, 240)),
                Background = isBorrowed ? new SolidColorBrush(Color.FromRgb(245, 245, 245)) : Brushes.White,
                Cursor = Cursors.Hand,
                Tag = book
            };

            // 借出时整体变灰的效果
            if (isBorrowed)
            {
                card.Opacity = 0.55;
            }

            var panel = new StackPanel();

            // 书名
            var titleBlock = new TextBlock
            {
                Text = book.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = isBorrowed ? new SolidColorBrush(Color.FromRgb(158, 158, 158)) : new SolidColorBrush(Color.FromRgb(33, 33, 33)),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 0, 0, 6)
            };
            panel.Children.Add(titleBlock);

            // 作者
            var authorBlock = new TextBlock
            {
                Text = $"作者：{book.Author}",
                FontSize = 13,
                Foreground = isBorrowed ? new SolidColorBrush(Color.FromRgb(158, 158, 158)) : new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Margin = new Thickness(0, 0, 0, 3)
            };
            panel.Children.Add(authorBlock);

            // 分类
            var categoryBlock = new TextBlock
            {
                Text = $"分类：{book.Category}",
                FontSize = 12,
                Foreground = isBorrowed ? new SolidColorBrush(Color.FromRgb(158, 158, 158)) : new SolidColorBrush(Color.FromRgb(136, 136, 136)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(categoryBlock);

            // 状态标签
            var statusBlock = new TextBlock
            {
                FontSize = 12,
                Margin = new Thickness(0, 4, 0, 0)
            };

            if (isBorrowed)
            {
                statusBlock.Text = $"🔒 已借出 - {book.Borrower}";
                statusBlock.Foreground = new SolidColorBrush(Color.FromRgb(239, 83, 80));
            }
            else
            {
                statusBlock.Text = "✅ 在馆";
                statusBlock.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            panel.Children.Add(statusBlock);

            // 操作按钮
            if (isBorrowed)
            {
                var returnBtn = new Button
                {
                    Content = "归还",
                    Width = 70,
                    Height = 28,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 10, 0, 0),
                    Tag = book
                };
                returnBtn.Click += BtnReturnBook_Click;
                panel.Children.Add(returnBtn);
            }
            else
            {
                var borrowBtn = new Button
                {
                    Content = "借阅",
                    Width = 70,
                    Height = 28,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 10, 0, 0),
                    Tag = book
                };
                borrowBtn.Click += BtnBorrowBook_Click;
                panel.Children.Add(borrowBtn);
            }

            card.Child = panel;
            return card;
        }

        /// <summary>
        /// 分类筛选变更
        /// </summary>
        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                RefreshBookList();
        }

        /// <summary>
        /// 点击录入新书
        /// </summary>
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            TxtNewTitle.Text = "";
            TxtNewAuthor.Text = "";
            TxtNewISBN.Text = "";
            CmbNewCategory.SelectedIndex = 0;
            TxtAddMessage.Visibility = Visibility.Collapsed;
            AddBookPopup.IsOpen = true;
        }

        /// <summary>
        /// 取消录入
        /// </summary>
        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            AddBookPopup.IsOpen = false;
        }

        /// <summary>
        /// 确认录入新书
        /// </summary>
        private void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtNewTitle.Text.Trim();
            string author = TxtNewAuthor.Text.Trim();
            string isbn = TxtNewISBN.Text.Trim();
            string category = CmbNewCategory.SelectedItem as string;

            if (string.IsNullOrEmpty(title))
            {
                ShowAddMessage("请输入书名", false);
                return;
            }
            if (string.IsNullOrEmpty(author))
            {
                ShowAddMessage("请输入作者", false);
                return;
            }
            if (string.IsNullOrEmpty(category))
            {
                ShowAddMessage("请选择分类", false);
                return;
            }

            var newBook = new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn,
                Category = category
            };

            BookManager.Instance.AddBook(newBook);
            AddBookPopup.IsOpen = false;
            RefreshBookList();

            MessageBox.Show($"《{title}》录入成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAddMessage(string msg, bool isSuccess)
        {
            TxtAddMessage.Text = msg;
            TxtAddMessage.Foreground = isSuccess
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Color.FromRgb(244, 67, 54));
            TxtAddMessage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 借阅图书
        /// </summary>
        private void BtnBorrowBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var book = btn?.Tag as Book;
            if (book == null) return;

            _selectedBook = book;
            TxtBorrowBookInfo.Text = $"《{book.Title}》\n作者：{book.Author}";
            TxtBorrower.Text = "";
            BorrowPopup.IsOpen = true;
        }

        /// <summary>
        /// 取消借阅
        /// </summary>
        private void BtnCancelBorrow_Click(object sender, RoutedEventArgs e)
        {
            BorrowPopup.IsOpen = false;
            _selectedBook = null;
        }

        /// <summary>
        /// 确认借阅
        /// </summary>
        private void BtnConfirmBorrow_Click(object sender, RoutedEventArgs e)
        {
            string borrower = TxtBorrower.Text.Trim();
            if (string.IsNullOrEmpty(borrower))
            {
                MessageBox.Show("请输入借阅人姓名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_selectedBook != null)
            {
                BookManager.Instance.BorrowBook(_selectedBook, borrower);
                BorrowPopup.IsOpen = false;
                _selectedBook = null;
                RefreshBookList();

                MessageBox.Show($"《{borrower}》借阅成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 归还图书
        /// </summary>
        private void BtnReturnBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var book = btn?.Tag as Book;
            if (book == null) return;

            var result = MessageBox.Show(
                $"确认归还《{book.Title}》？\n借阅人：{book.Borrower}",
                "归还确认", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                BookManager.Instance.ReturnBook(book);
                RefreshBookList();

                MessageBox.Show($"《{book.Title}》已成功归还！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要退出登录吗？", "提示",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}