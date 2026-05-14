using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace WpfApp3
{
    /// <summary>
    /// 图书模型
    /// </summary>
    public class Book : INotifyPropertyChanged
    {
        private string _title;
        private string _author;
        private string _isbn;
        private string _category;
        private bool _isBorrowed;
        private string _borrower;
        private DateTime? _borrowDate;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Author
        {
            get => _author;
            set { _author = value; OnPropertyChanged(nameof(Author)); }
        }

        public string ISBN
        {
            get => _isbn;
            set { _isbn = value; OnPropertyChanged(nameof(ISBN)); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public bool IsBorrowed
        {
            get => _isBorrowed;
            set
            {
                _isBorrowed = value;
                OnPropertyChanged(nameof(IsBorrowed));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public string Borrower
        {
            get => _borrower;
            set { _borrower = value; OnPropertyChanged(nameof(Borrower)); }
        }

        public DateTime? BorrowDate
        {
            get => _borrowDate;
            set { _borrowDate = value; OnPropertyChanged(nameof(BorrowDate)); }
        }

        public string StatusText => IsBorrowed ? $"已借出 - {Borrower}" : "在馆";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// 图书管理器（单例，内存存储）
    /// </summary>
    public class BookManager
    {
        private static BookManager _instance;
        public static BookManager Instance => _instance ?? (_instance = new BookManager());

        public ObservableCollection<Book> Books { get; } = new ObservableCollection<Book>();

        /// <summary>
        /// 预设图书分类
        /// </summary>
        public string[] Categories => new[] { "全部", "文学小说", "科学技术", "历史传记", "哲学思想", "艺术设计", "经济管理", "教育学习", "其他" };

        private BookManager()
        {
            // 预置一些示例图书
            Books.Add(new Book { Title = "红楼梦", Author = "曹雪芹", ISBN = "978-7-02-000220-0", Category = "文学小说" });
            Books.Add(new Book { Title = "三体", Author = "刘慈欣", ISBN = "978-7-5366-9293-0", Category = "科学技术" });
            Books.Add(new Book { Title = "百年孤独", Author = "加西亚·马尔克斯", ISBN = "978-7-5442-4528-0", Category = "文学小说" });
            Books.Add(new Book { Title = "史记", Author = "司马迁", ISBN = "978-7-101-00304-1", Category = "历史传记" });
            Books.Add(new Book { Title = "C# 本质论", Author = "Ben Albahari", ISBN = "978-7-115-41682-4", Category = "科学技术", IsBorrowed = true, Borrower = "张三", BorrowDate = DateTime.Now.AddDays(-3) });
            Books.Add(new Book { Title = "人类简史", Author = "尤瓦尔·赫拉利", ISBN = "978-7-5086-4735-1", Category = "历史传记" });
            Books.Add(new Book { Title = "论语", Author = "孔子", ISBN = "978-7-101-00352-6", Category = "哲学思想" });
            Books.Add(new Book { Title = "艺术的故事", Author = "贡布里希", ISBN = "978-7-5495-0965-5", Category = "艺术设计" });
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
        }

        public void BorrowBook(Book book, string borrower)
        {
            book.IsBorrowed = true;
            book.Borrower = borrower;
            book.BorrowDate = DateTime.Now;
        }

        public void ReturnBook(Book book)
        {
            book.IsBorrowed = false;
            book.Borrower = null;
            book.BorrowDate = null;
        }

        public ObservableCollection<Book> GetBooksByCategory(string category)
        {
            if (category == "全部" || string.IsNullOrEmpty(category))
                return Books;

            var filtered = new ObservableCollection<Book>();
            foreach (var book in Books)
            {
                if (book.Category == category)
                    filtered.Add(book);
            }
            return filtered;
        }
    }
}