using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibraryWpfClient.Services;
using LibraryGrpcService;
using Grpc.Core;

namespace LibraryWpfClient
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<BookDisplay> _books = new();
        private List<BookDisplay> _allBooks = new();

        public MainWindow()
        {
            InitializeComponent();
            DgBooks.ItemsSource = _books;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var currentUser = Application.Current.Properties["CurrentUser"]?.ToString() ?? "";
            TxtCurrentUser.Text = $"当前用户: {currentUser}";

            // 初始化分类下拉框
            var categories = new[] { "全部", "文学", "科技", "历史", "教育", "艺术", "其他" };
            CmbCategory.ItemsSource = categories;
            CmbCategory.SelectedIndex = 0;
            CmbNewCategory.ItemsSource = categories.Where(c => c != "全部").ToList();
            CmbNewCategory.SelectedIndex = 0;

            await LoadBooksAsync();
        }

        private async Task LoadBooksAsync(string category = "")
        {
            try
            {
                var client = GrpcClientService.CreateBookClient();
                var response = await client.GetBooksAsync(new GetBooksRequest
                {
                    Category = category ?? ""
                });

                _allBooks = response.Books.Select(b => new BookDisplay
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Isbn = b.Isbn,
                    Category = b.Category,
                    IsBorrowed = b.IsBorrowed,
                    Borrower = b.Borrower,
                    BorrowDate = b.BorrowDate
                }).ToList();

                RefreshBooksList();
                TxtStatus.Text = $"共 {_allBooks.Count} 本图书";
            }
            catch (RpcException ex)
            {
                TxtStatus.Text = $"加载失败：{ex.Status.Detail}";
                MessageBox.Show($"连接服务器失败：{ex.Status.Detail}\n请确保服务端已启动。", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"加载失败：{ex.Message}";
            }
        }

        private void RefreshBooksList(string keyword = "")
        {
            var filtered = string.IsNullOrWhiteSpace(keyword)
                ? _allBooks
                : _allBooks.Where(b =>
                    b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    b.Isbn.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            _books.Clear();
            foreach (var book in filtered)
                _books.Add(book);
        }

        private async void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCategory.SelectedItem is string category)
                await LoadBooksAsync(category == "全部" ? "" : category);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            RefreshBooksList(TxtSearch.Text.Trim());
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            TxtNewTitle.Text = "";
            TxtNewAuthor.Text = "";
            TxtNewIsbn.Text = "";
            CmbNewCategory.SelectedIndex = 0;
            TxtAddMessage.Visibility = Visibility.Collapsed;
            AddBookPanel.Visibility = Visibility.Visible;
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            AddBookPanel.Visibility = Visibility.Collapsed;
        }

        private async void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtNewTitle.Text.Trim();
            string author = TxtNewAuthor.Text.Trim();
            string isbn = TxtNewIsbn.Text.Trim();
            string category = CmbNewCategory.SelectedItem?.ToString() ?? "其他";

            if (string.IsNullOrEmpty(title))
            {
                TxtAddMessage.Text = "请输入书名！";
                TxtAddMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var client = GrpcClientService.CreateBookClient();
                var book = await client.AddBookAsync(new AddBookRequest
                {
                    Title = title,
                    Author = author,
                    Isbn = isbn,
                    Category = category
                });

                AddBookPanel.Visibility = Visibility.Collapsed;
                await LoadBooksAsync(CmbCategory.SelectedItem?.ToString() == "全部" ? "" : CmbCategory.SelectedItem?.ToString());
                TxtStatus.Text = $"已添加图书：{book.Title}";
            }
            catch (RpcException ex)
            {
                TxtAddMessage.Text = $"添加失败：{ex.Status.Detail}";
                TxtAddMessage.Visibility = Visibility.Visible;
            }
        }

        private async void BtnAction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookDisplay book)
            {
                try
                {
                    var client = GrpcClientService.CreateBookClient();

                    if (book.IsBorrowed)
                    {
                        // 归还
                        await client.ReturnBookAsync(new ReturnBookRequest { BookId = book.Id });
                        TxtStatus.Text = $"已归还：{book.Title}";
                    }
                    else
                    {
                        // 借阅 - 弹出输入框
                        string borrower = Application.Current.Properties["CurrentUser"]?.ToString() ?? "未知用户";
                        await client.BorrowBookAsync(new BorrowBookRequest
                        {
                            BookId = book.Id,
                            Borrower = borrower
                        });
                        TxtStatus.Text = $"已借阅：{book.Title}";
                    }

                    await LoadBooksAsync(CmbCategory.SelectedItem?.ToString() == "全部" ? "" : CmbCategory.SelectedItem?.ToString());
                }
                catch (RpcException ex)
                {
                    MessageBox.Show($"操作失败：{ex.Status.Detail}", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookDisplay book)
            {
                var result = MessageBox.Show($"确定要删除《{book.Title}》吗？", "确认删除",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var client = GrpcClientService.CreateBookClient();
                        await client.DeleteBookAsync(new DeleteBookRequest { Id = book.Id });
                        TxtStatus.Text = $"已删除：{book.Title}";
                        await LoadBooksAsync(CmbCategory.SelectedItem?.ToString() == "全部" ? "" : CmbCategory.SelectedItem?.ToString());
                    }
                    catch (RpcException ex)
                    {
                        MessageBox.Show($"删除失败：{ex.Status.Detail}", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            GrpcClientService.Shutdown();
            Application.Current.Properties["CurrentUser"] = null;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }

    /// <summary>
    /// 图书显示模型
    /// </summary>
    public class BookDisplay
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Isbn { get; set; } = "";
        public string Category { get; set; } = "";
        public bool IsBorrowed { get; set; }
        public string Borrower { get; set; } = "";
        public string BorrowDate { get; set; } = "";

        public string Status => IsBorrowed ? "已借出" : "可借阅";
        public string ActionText => IsBorrowed ? "归还" : "借阅";
    }
}