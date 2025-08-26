# 🧠 JeeBeginner Backend API

## 🎯 Tổng quan chức năng

- **Quản lý tài khoản**: Đăng nhập, phân quyền, quản lý người dùng
- **Quản lý kho**: Mặt hàng, loại mặt hàng, đơn vị tính
- **Quản lý khách hàng**: CRUD khách hàng và ứng dụng
- **Dashboard**: Widget và báo cáo
- **Menu động**: Hệ thống menu theo phân quyền

## 🛠 Công nghệ sử dụng

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server
- **Authentication**: JWT Bearer Token
- **ORM**: ADO.NET với DpsLibs
- **Documentation**: Swagger/OpenAPI
- **Containerization**: Docker
- **Cloud**: Azure App Service

---

## 📁 Cấu trúc dự án
<pre lang="markdown"> ``` AngularBackEnd/ ├── Classes/ # Các lớp tiện ích │ ├── Constant.cs │ ├── JsonResultCommon.cs │ ├── MessageReturnHelper.cs │ └── Common.cs ├── Controllers/ # API Controllers │ ├── AccountManagementController.cs │ ├── InventoryController.cs │ ├── MenuController.cs │ └── WidgetDashBoardController.cs ├── Models/ # Data models │ ├── Common/ │ ├── AccountManagement/ │ └── InventoryManagement/ ├── Services/ # Business logic │ ├── AccountManagement/ │ ├── Authorization/ │ └── CustomerManagement/ ├── Repositories/ # Data access layer ├── MiddleWare/ # Custom middleware │ ├── CheckPermission.cs │ ├── ErrorHandlingMiddleware.cs │ └── LoggingMiddleware.cs ├── DPS/ # External libraries (DLLs) └── wwwroot/uploads/ # File storage ``` </pre>

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống

- [.NET SDK 9.0](https://dotnet.microsoft.com/)
- SQL Server 2019 trở lên
- Visual Studio 2022 hoặc VS Code

### Cài đặt

```bash
git clone https://github.com/your-org/AngularBackEnd.git
cd AngularBackEnd
dotnet restore

⚙️ Cấu hình appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=JeeBeginner;User=sa;Password=YourPassword;TrustServerCertificate=true;"
  },
  "JWT": {
    "Secret": "YourSuperSecretJWTKeyHere123456789012345678901234567890"
  },
  "AllowedHosts": "*"
}

🔓 Cấu hình CORS

Trong Program.cs:
builder.Services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

🏃‍♂️ Chạy ứng dụng

Development
dotnet run

Production
dotnet run --environment Production

Ứng dụng sẽ chạy tại:

HTTP: http://localhost:5000
HTTPS: https://localhost:5001
Swagger: http://localhost:5000/swagger
