using System.Net.Http;
using Grpc.Net.Client;
using LibraryGrpcService;

namespace LibraryWpfClient.Services;

/// <summary>
/// gRPC 客户端服务，提供通道和客户端创建
/// </summary>
public static class GrpcClientService
{
    private static GrpcChannel? _channel;

    public static GrpcChannel GetChannel()
    {
        if (_channel == null)
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _channel = GrpcChannel.ForAddress(App.ServerAddress, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
        }
        return _channel;
    }

    public static BookService.BookServiceClient CreateBookClient()
    {
        return new BookService.BookServiceClient(GetChannel());
    }

    public static UserService.UserServiceClient CreateUserClient()
    {
        return new UserService.UserServiceClient(GetChannel());
    }

    public static void Shutdown()
    {
        _channel?.Dispose();
        _channel = null;
    }
}