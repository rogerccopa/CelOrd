using System;
using Microsoft.EntityFrameworkCore;

namespace CelOrdApp.Data;

public class ClientDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientDbContextFactory(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbContextOptions<ClientDbContext> CreateDbContextOptions()
    {
        string dbName = GetDbNameFromClaims();
        if (string.IsNullOrEmpty(dbName))
        {
            throw new InvalidOperationException("Database name not found in claims.");
        }
        
        var connectionString = _configuration.GetConnectionString("ClientDbConnStr")?.Replace("co_base", dbName);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("ClientDbConnStr not found in appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return optionsBuilder.Options;
    }

    private string GetDbNameFromClaims()
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims;
        var dbName = claims?.FirstOrDefault(c => c.Type == "dbName")?.Value;

        if (string.IsNullOrEmpty(dbName))
        {
            throw new InvalidOperationException("dbName claim not found.");
        }

        return dbName;
    }
}
