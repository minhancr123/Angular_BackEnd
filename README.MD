🎯 Tổng quan
API backend cung cấp các chức năng:
•	Quản lý tài khoản: Đăng nhập, phân quyền, quản lý người dùng
•	Quản lý kho: Quản lý mặt hàng, loại mặt hàng, đơn vị tính
•	Quản lý khách hàng: CRUD khách hàng và ứng dụng
•	Dashboard: Widget và báo cáo
•	Menu động: Hệ thống menu theo phân quyền
🛠 Công nghệ sử dụng
•	Framework: ASP.NET Core 9.0
•	Database: SQL Server
•	Authentication: JWT Bearer Token
•	ORM: ADO.NET với DpsLibs
•	Documentation: Swagger/OpenAPI
•	Containerization: Docker
•	Cloud: Azure App Service
📁 Cấu trúc dự án
AngularBackEnd/
├── 📂 Classes/                 # Các lớp tiện ích
│   ├── Constant.cs            # Hằng số và cấu hình
│   ├── JsonResultCommon.cs    # Format response chung
│   ├── MessageReturnHelper.cs # Helper messages
│   └── Common.cs              # Utility functions
├── 📂 Controllers/            # API Controllers
│   ├── AccountManagementController.cs
│   ├── InventoryController.cs
│   ├── MenuController.cs
│   └── WidgetDashBoardController.cs
├── 📂 Models/                 # Data models
│   ├── Common/                # Base models
│   ├── AccountManagement/
│   └── InventoryManagement/
├── 📂 Services/               # Business logic
│   ├── AccountManagement/
│   ├── Authorization/
│   └── CustomerManagement/
├── 📂 Repositories/           # Data access layer
├── 📂 MiddleWare/            # Custom middleware
│   ├── CheckPermission.cs     # Authorization middleware
│   ├── ErrorHandlingMiddleware.cs
│   └── LoggingMiddleware.cs
├── 📂 DPS/                   # External libraries
│   ├── DBConnection.dll
│   ├── DpsLibs.dll
│   └── DPSinfra.dll
└── 📂 wwwroot/uploads/       # File storage


🚀 Cài đặt
Yêu cầu hệ thống
•	.NET 9.0 SDK
•	SQL Server 2019+
•	Visual Studio 2022 hoặc VS Code

Clone dự án
git clone <repository-url>
cd AngularBackEnd

Cài đặt dependencies
dotnet restore

⚙️ Cấu hình
1. Cấu hình Database
Tạo file appsettings.json:
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

2. Cấu hình CORS
CORS đã được cấu hình trong Program.cs để cho phép tất cả origins:
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
•	HTTP: http://localhost:5000
•	HTTPS: https://localhost:5001
•	Swagger UI: http://localhost:5000/swagger
