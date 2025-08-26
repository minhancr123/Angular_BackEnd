# Sử dụng base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Sử dụng SDK image để build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file và restore dependencies
COPY ["AngularBackEnd.csproj", "."]
RUN dotnet restore "./AngularBackEnd.csproj"

# Copy toàn bộ source code
COPY . .
WORKDIR "/src/."
RUN dotnet build "./AngularBackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AngularBackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Tạo thư mục uploads để lưu file
RUN mkdir -p /app/wwwroot/uploads

# Copy published files
COPY --from=publish /app/publish .

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AngularBackEnd.dll"]