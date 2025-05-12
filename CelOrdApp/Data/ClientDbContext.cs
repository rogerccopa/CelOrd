using System.Security.Claims;
using CelOrdApp.Models;
using Microsoft.EntityFrameworkCore;
using static Domain.EntityTypes;

namespace CelOrdApp.Data;

public class ClientDbContext : DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }

	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly AppParams _appParams;

	public ClientDbContext(
		DbContextOptions<ClientDbContext> dbCtxOptions,
		IHttpContextAccessor httpContextAccessor,
		AppParams appParams) : base(dbCtxOptions)
	{
		_httpContextAccessor = httpContextAccessor;
		_appParams = appParams;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		ClaimsPrincipal? userClaims = _httpContextAccessor.HttpContext?.User;

		if (userClaims != null)
		{
			if (userClaims.Identity?.IsAuthenticated == true)
			{
				string clientDbName = userClaims.FindFirst("dbName")?.Value ?? string.Empty;
				string dbConnStr = _appParams.ClientBaseDbConnStr;

				if (!string.IsNullOrEmpty(clientDbName))
				{
					dbConnStr = dbConnStr.Replace("Database=co_base;", $"Database={clientDbName};");
					optionsBuilder.UseSqlServer(dbConnStr);
				}
			}
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configuring the User entity
		modelBuilder.Entity<User>(user =>
		{
			user.HasKey(u => u.Id);
			user.Property(u => u.Id).ValueGeneratedOnAdd();
			user.Property(u => u.FullName).HasMaxLength(30);
			user.Property(u => u.Username).IsRequired().HasMaxLength(30);
			user.Property(u => u.Password).IsRequired().HasMaxLength(150);
			user.Property(u => u.Areas).HasMaxLength(30).HasConversion(
					v => string.Join(',', v.Select(a => a.ToString())),
					v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
						.Select(a => (Area)Enum.Parse(typeof(Area), a)).ToList());
			user.Property(u => u.CreatedAt).HasDefaultValueSql("getdate()");
			user.Property(u => u.Claims).HasConversion(
					v => string.Join(',', v.Select(c => c.ToString())),
					v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
						.Select(c => (UserClaim)Enum.Parse(typeof(UserClaim), c)).ToList());
		});
		modelBuilder.Entity<User>().Ignore(u => u.Claims); // Ignore Claims property in the database mapping

		// Configuring the Product entity
		modelBuilder.Entity<Product>(product =>
		{
			product.HasKey(p => p.Id);
			product.Property(p => p.Id).ValueGeneratedOnAdd();
			product.Property(p => p.Title).IsRequired().HasMaxLength(30);
			product.Property(p => p.Description).HasMaxLength(90);
			product.Property(p => p.Price).IsRequired().HasPrecision(9, 2);
		});

		// Configuring the Order entity
		modelBuilder.Entity<Order>(order =>
		{
			order.HasKey(o => o.Id);
			order.Property(o => o.Id).ValueGeneratedOnAdd();
			order.Property(o => o.UserId).IsRequired();
			order.Property(o => o.TableName).HasMaxLength(30);
			order.Property(o => o.CreatedAt).HasDefaultValueSql("getdate()");
			order.Property(o => o.Total).HasPrecision(9, 2);
		});
		modelBuilder.Entity<Order>()
			.HasMany(o => o.OrderItems) // Specifies that an Order has many OrderItems (one-to-many relationship)
			.WithOne()  // Specifies that each OrderItem is related to one Order.
			.HasForeignKey(ordItem => ordItem.OrderId); // Configures the foreign key in OrderItem to link it to an Order.

		// Configuring the OrderItem entity
		modelBuilder.Entity<OrderItem>(ordItem =>
		{
			ordItem.HasKey(oi => oi.Id);
			ordItem.Property(oi => oi.Id).ValueGeneratedOnAdd();
			ordItem.Property(oi => oi.ProductId).IsRequired();
			ordItem.Property(oi => oi.Quantity).IsRequired();
			ordItem.Property(oi => oi.Price).IsRequired().HasPrecision(9, 2);
			ordItem.Property(oi => oi.Subtotal).HasPrecision(9, 2);
			ordItem.Property(oi => oi.Note).HasMaxLength(30);
		});

		base.OnModelCreating(modelBuilder);
	}
}
