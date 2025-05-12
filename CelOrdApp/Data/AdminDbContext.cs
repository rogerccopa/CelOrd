using CelOrdApp.Models;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Domain.EntityTypes;

namespace CelOrdApp.Data;

public class AdminDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }

    public AdminDbContext(DbContextOptions<AdminDbContext> dbCtxOptions) : base(dbCtxOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuring the Company entity
        modelBuilder.Entity<Company>(company =>
        {
            company.HasKey(c => c.Id);
            company.Property(c => c.Id).ValueGeneratedOnAdd();
            company.Property(c => c.CompanyName).IsRequired().HasMaxLength(30);
            company.Property(c => c.Subdomain).IsRequired().HasMaxLength(30);
            company.Property(c => c.DbName).HasMaxLength(6);
			company.Property(c => c.Email).IsRequired().HasMaxLength(30);
			company.Property(c => c.CreatedAt).HasDefaultValueSql("getdate()");
        });

        base.OnModelCreating(modelBuilder);
    }

    public string CreateDatabase(string newDbName, string createDbTablesSql)
    {
        string dbConnStr = this.Database.GetConnectionString()!;
        string query = $"CREATE DATABASE {newDbName}";

        using (var conn = new SqlConnection(dbConnStr))
        using (var cmd = new SqlCommand(query, conn))
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        dbConnStr = dbConnStr.Replace("Database=co_admin;", $"Database={newDbName};");
        using (var conn = new SqlConnection(dbConnStr))
        using (var cmd = new SqlCommand(createDbTablesSql, conn))
        {
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        return dbConnStr;
    }

    public void InsertAdminUser(string username, string password, string clientDbConnStr)
    {
        string query = $"INSERT INTO Users(FullName, Username, Password, Areas) " +
                        $"VALUES('Admin', @username, @password, '{Area.Admin.ToSpanish()}')";
        
        using (var conn = new SqlConnection(clientDbConnStr))
        using (var cmd = new SqlCommand(query, conn))
        {
            conn.Open();

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();
        }
    }

    public CelOrdAccount GetAccount(string dbName)
    {
        string dbConnStr = this.Database.GetConnectionString()!;

        string query = $"SELECT Id, CompanyName, Subdomain, DbName FROM Companies WHERE DbName='{dbName}'";
        using (var conn = new SqlConnection(dbConnStr))
        using (var cmd = new SqlCommand(query, conn))
        {
            conn.Open();

            var reader = cmd.ExecuteReader();

            if (!reader.Read())
                throw new Exception($"Entidad {dbName} no encontrado en la base de datos");

            return new CelOrdAccount()
            {
                Id = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                Subdomain = reader.GetString(2),
                DbName = reader.GetString(3)
            };
        }
    }
}
