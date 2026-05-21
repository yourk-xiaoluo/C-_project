using Grpc.Core;
using LibraryGrpcService.Data;
using LibraryGrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryGrpcService.Services;

public class BookGrpcService : BookService.BookServiceBase
{
    private readonly LibraryDbContext _db;
    private readonly ILogger<BookGrpcService> _logger;

    public BookGrpcService(LibraryDbContext db, ILogger<BookGrpcService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task<GetBooksResponse> GetBooks(GetBooksRequest request, ServerCallContext context)
    {
        IQueryable<BookEntity> query = _db.Books;

        if (!string.IsNullOrEmpty(request.Category) && request.Category != "全部")
        {
            query = query.Where(b => b.Category == request.Category);
        }

        var books = await query.ToListAsync(context.CancellationToken);

        var response = new GetBooksResponse();
        response.Books.AddRange(books.Select(MapToMessage));

        return response;
    }

    public override async Task<BookMessage> GetBook(GetBookRequest request, ServerCallContext context)
    {
        var book = await _db.Books.FindAsync(new object[] { request.Id }, context.CancellationToken);
        if (book == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"图书 ID={request.Id} 不存在"));
        }
        return MapToMessage(book);
    }

    public override async Task<BookMessage> AddBook(AddBookRequest request, ServerCallContext context)
    {
        var entity = new BookEntity
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.Isbn,
            Category = request.Category
        };

        _db.Books.Add(entity);
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("添加图书: {Title}", entity.Title);
        return MapToMessage(entity);
    }

    public override async Task<BookMessage> BorrowBook(BorrowBookRequest request, ServerCallContext context)
    {
        var book = await _db.Books.FindAsync(new object[] { request.BookId }, context.CancellationToken);
        if (book == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"图书 ID={request.BookId} 不存在"));
        }
        if (book.IsBorrowed)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, $"图书 \"{book.Title}\" 已被借出"));
        }

        book.IsBorrowed = true;
        book.Borrower = request.Borrower;
        book.BorrowDate = DateTime.Now;
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("借阅图书: {Title}, 借阅人: {Borrower}", book.Title, request.Borrower);
        return MapToMessage(book);
    }

    public override async Task<BookMessage> ReturnBook(ReturnBookRequest request, ServerCallContext context)
    {
        var book = await _db.Books.FindAsync(new object[] { request.BookId }, context.CancellationToken);
        if (book == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"图书 ID={request.BookId} 不存在"));
        }
        if (!book.IsBorrowed)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, $"图书 \"{book.Title}\" 未被借出"));
        }

        book.IsBorrowed = false;
        book.Borrower = null;
        book.BorrowDate = null;
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("归还图书: {Title}", book.Title);
        return MapToMessage(book);
    }

    public override async Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request, ServerCallContext context)
    {
        var book = await _db.Books.FindAsync(new object[] { request.Id }, context.CancellationToken);
        if (book == null)
        {
            return new DeleteBookResponse { Success = false };
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("删除图书: {Title}", book.Title);
        return new DeleteBookResponse { Success = true };
    }

    private static BookMessage MapToMessage(BookEntity entity)
    {
        return new BookMessage
        {
            Id = entity.Id,
            Title = entity.Title,
            Author = entity.Author,
            Isbn = entity.ISBN,
            Category = entity.Category,
            IsBorrowed = entity.IsBorrowed,
            Borrower = entity.Borrower ?? string.Empty,
            BorrowDate = entity.BorrowDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty
        };
    }
}