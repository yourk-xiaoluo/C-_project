# 📚 图书管理系统

一个基于 **C# WPF + gRPC** 的客户端/服务端架构图书管理系统，提供图书的录入、借阅、归还、分类筛选等核心功能，采用现代化的 UI 界面设计，数据持久化存储在 SQLite 数据库中。

## ✨ 功能特性

### 用户管理
- **用户注册**：支持新用户注册账号（需填写用户名、密码、邮箱）
- **用户登录**：账号密码登录验证
- **密码重置**：支持通过用户名和邮箱验证身份后重置密码
- **退出登录**：安全退出当前账户

### 图书管理
- **图书录入**：录入新图书，填写书名、作者、ISBN、分类等信息
- **图书展示**：以卡片形式展示图书信息，直观清晰
- **图书搜索**：支持按书名、作者、ISBN 模糊搜索
- **分类筛选**：支持按分类（文学、科技、历史、教育、艺术、其他）筛选图书
- **图书删除**：支持删除指定图书
- **图书统计**：实时显示当前分类下的图书总数

### 借阅管理
- **图书借阅**：自动记录借阅人姓名和借阅日期
- **图书归还**：归还已借出的图书，自动清除借阅记录
- **状态标识**：借出的图书显示「已借出」标签，可借图书显示「可借阅」标签
- **借阅信息**：实时显示借阅人和借阅状态

## 🛠️ 技术栈

| 技术 | 说明 |
|------|------|
| **C#** | 主要编程语言 |
| **.NET 10.0** | 运行框架 |
| **WPF** | Windows 桌面客户端框架（UI 层） |
| **XAML** | UI 界面标记语言 |
| **gRPC** | 客户端与服务端通信协议 |
| **Protocol Buffers** | 接口定义语言（.proto 文件） |
| **ASP.NET Core** | 服务端 Web 框架，托管 gRPC 服务 |
| **Entity Framework Core** | ORM 框架，操作数据库 |
| **SQLite** | 轻量级关系型数据库 |
| **依赖注入 (DI)** | ASP.NET Core 内置依赖注入容器 |

### 架构概览

```
┌───────────────────────────────┐       gRPC / HTTPS        ┌─────────────────────────────────┐
│     WPF 客户端 (net10.0)      │ ◄────────────────────────► │     ASP.NET Core 服务端          │
│     LibraryWpfClient          │      Protobuf 序列化       │     LibraryGrpcService           │
│                               │                            │                                  │
│  ┌───────────────────────┐    │                            │  ┌──────────────────────────┐    │
│  │ LoginWindow            │   │                            │  │ UserService (用户服务)     │    │
│  │ RegisterWindow         │   │                            │  │ - 注册、登录              │    │
│  │ ResetPasswordWindow    │   │                            │  │ - 邮箱验证               │    │
│  │ MainWindow (图书管理)   │   │                            │  │ - 密码重置               │    │
│  └───────────────────────┘    │                            │  └──────────────────────────┘    │
│  ┌───────────────────────┐    │                            │  ┌──────────────────────────┐    │
│  │ GrpcClientService      │   │                            │  │ BookService (图书服务)     │    │
│  │ (gRPC 客户端封装)       │   │                            │  │ - 图书 CRUD              │    │
│  └───────────────────────┘    │                            │  │ - 借阅/归还              │    │
│                               │                            │  └──────────────────────────┘    │
└───────────────────────────────┘                            │          │                        │
                                                             │  ┌───────▼──────────────────┐    │
                                                             │  │ EF Core + SQLite         │    │
                                                             │  │ (数据持久化)              │    │
                                                             │  └──────────────────────────┘    │
                                                             └─────────────────────────────────┘
```

## 📁 项目结构

```
图书管理系统/
├── WpfApp3.slnx                         # 解决方案文件
├── .gitignore                            # Git 忽略配置
├── README.md                             # 项目说明文档
│
├── LibraryGrpcService/                   # ── gRPC 服务端 ──
│   ├── Program.cs                        # 服务端入口 & gRPC 服务注册
│   ├── appsettings.json                  # 服务端配置
│   ├── LibraryGrpcService.csproj         # 服务端项目文件
│   ├── Protos/
│   │   ├── book.proto                    # 图书服务接口定义
│   │   └── user.proto                    # 用户服务接口定义
│   ├── Services/
│   │   ├── BookGrpcService.cs            # 图书 gRPC 服务实现
│   │   └── UserGrpcService.cs            # 用户 gRPC 服务实现
│   ├── Models/
│   │   ├── BookEntity.cs                 # 图书数据库实体
│   │   └── UserEntity.cs                 # 用户数据库实体
│   ├── Data/
│   │   └── LibraryDbContext.cs           # EF Core 数据库上下文
│   ├── Properties/
│   │   └── launchSettings.json           # 启动配置（端口等）
│   └── library.db                        # SQLite 数据库文件（运行后生成）
│
└── LibraryWpfClient/                     # ── WPF 客户端 ──
    ├── App.xaml / App.xaml.cs            # 应用程序入口 & 服务器地址配置
    ├── LoginWindow.xaml / .cs            # 登录窗口
    ├── RegisterWindow.xaml / .cs         # 注册窗口
    ├── ResetPasswordWindow.xaml / .cs    # 密码重置窗口
    ├── MainWindow.xaml / .cs             # 主窗口（图书管理界面）
    ├── Services/
    │   └── GrpcClientService.cs          # gRPC 客户端封装服务
    ├── AssemblyInfo.cs                   # 程序集信息
    └── LibraryWpfClient.csproj           # 客户端项目文件
```

## 🚀 快速开始

### 环境要求

- **操作系统**：Windows 10 / Windows 11
- **开发工具**：Visual Studio 2022 或更高版本
- **.NET SDK**：.NET 10.0 或更高版本

### 安装 .NET SDK

前往 [dotnet.microsoft.com](https://dotnet.microsoft.com/download) 下载安装 .NET 10.0 SDK。

### 运行步骤

1. **克隆仓库**
   ```bash
   git clone https://github.com/yourk-xiaoluo/C-_project.git
   ```

2. **还原依赖**
   ```bash
   cd 图书管理系统
   dotnet restore WpfApp3.slnx
   ```

3. **启动 gRPC 服务端**
   ```bash
   # 在一个终端窗口中启动服务端
   cd LibraryGrpcService
   dotnet run
   ```
   服务端默认运行在 `https://localhost:7212`。

4. **启动 WPF 客户端**
   ```bash
   # 在另一个终端窗口中启动客户端（或在 Visual Studio 中直接运行）
   cd LibraryWpfClient
   dotnet run
   ```

   > **也可以使用 Visual Studio**：打开 `WpfApp3.slnx`，将 `LibraryGrpcService` 和 `LibraryWpfClient` 均设为启动项目，按 `F5` 即可同时启动。

### 服务器地址配置

客户端默认连接 `https://localhost:7212`。如需修改，可在 `LibraryWpfClient/App.xaml.cs` 中更改：
```csharp
public static string ServerAddress { get; set; } = "https://localhost:7212";
```

## 📖 使用说明

1. 启动应用后，首先进入**登录界面**
2. 新用户需要先**注册账号**（填写用户名、密码、邮箱）
3. 登录成功后进入**图书管理主界面**
4. 在主界面可以：
   - 浏览所有图书卡片列表
   - 通过顶部搜索框按**书名/作者/ISBN**搜索图书
   - 通过下拉框**筛选**不同分类的图书
   - 点击**「录入图书」**按钮添加新图书
   - 点击图书列表中的**「借阅」**按钮借阅图书
   - 点击已借出图书的**「归还」**按钮归还图书
   - 点击**「删除」**按钮删除图书（需确认）
   - 点击右上角**「退出登录」**返回登录界面
5. 忘记密码可在登录界面点击**「忘记密码」**，通过用户名和邮箱验证后重置

## 📄 许可证

本项目仅供学习交流使用。