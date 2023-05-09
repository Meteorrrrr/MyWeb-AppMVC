# CLI Tools
- Cài đặt lệnh ```dotnet aspnet-codegenerator```
```
dotnet tool install -g dotnet-aspnet-codegenerator

dotnet tool install --global dotnet-ef
dotnet tool update dotnet-ef
```

# Packages
```
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet
```
## Enable local HTTPS
```
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```
