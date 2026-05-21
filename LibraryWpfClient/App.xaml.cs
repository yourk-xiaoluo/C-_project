using System.Windows;

namespace LibraryWpfClient
{
    public partial class App : Application
    {
        // gRPC 服务器地址
        public static string ServerAddress { get; set; } = "https://localhost:7212";
    }
}