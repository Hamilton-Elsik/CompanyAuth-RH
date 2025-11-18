dotnet ef migrations add InitialCreate -p CompanyAuth.Infrastructure -s CompanyAuth.Api
dotnet ef database update -p CompanyAuth.Infrastructure -s CompanyAuth.Api
