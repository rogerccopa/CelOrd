using CelOrdApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CelOrdApp.Data;

public class ClientDbContext :DbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }

	private readonly DbContextOptions<ClientDbContext> _dbCtxOptions;

	public ClientDbContext(DbContextOptions<ClientDbContext> dbCtxOptions) : base(dbCtxOptions)
	{
		_dbCtxOptions = dbCtxOptions;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configuring the User entity
		modelBuilder.Entity<User>()
			.Property(user => user.FullName)
			.HasMaxLength(30);
		modelBuilder.Entity<User>()
			.Property(user => user.Username)
			.HasMaxLength(30);
		modelBuilder.Entity<User>()
			.Property(user => user.Password)
			.HasMaxLength(150);
		modelBuilder.Entity<User>()
			.Property(entity => entity.CreatedAt)
			.HasDefaultValueSql("getdate()");
		modelBuilder.Entity<User>()
			.Property(user => user.Claims)
			.HasConversion(
				v => string.Join(',', v.Select(c => c.ToString())),
				v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(c => (UserClaim)Enum.Parse(typeof(UserClaim), c)).ToList());
		modelBuilder.Entity<User>().Ignore(user => user.Claims);

		// Configuring the Product entity
		modelBuilder.Entity<Product>()
			.Property(prod => prod.Title)
			.HasMaxLength(30);
		modelBuilder.Entity<Product>()
			.Property(prod => prod.Description)
			.HasMaxLength(90);
		modelBuilder.Entity<Product>()
			.Property(prod => prod.Price)
			.HasPrecision(9, 2);

		// Configuring the Order entity
		modelBuilder.Entity<Order>()
			.Property(order => order.TableName)
			.HasMaxLength(30);
		modelBuilder.Entity<Order>()
			.Property(order => order.CreatedAt)
			.HasDefaultValueSql("getdate()");
		modelBuilder.Entity<Order>()
			.Property(order => order.Total)
			.HasPrecision(9, 2);
		modelBuilder.Entity<Order>()
			.HasMany(ord => ord.OrderItems) // Specifies that an Order has many OrderItems (one-to-many relationship)
			.WithOne()  // Specifies that each OrderItem is related to one Order.
			.HasForeignKey(ordItem => ordItem.OrderId);   // Configures the foreign key in OrderItem that links it to an Order.

		// Configuring the OrderItem entity
		modelBuilder.Entity<OrderItem>()
			.Property(item => item.OrderId)
			.IsRequired();
		modelBuilder.Entity<OrderItem>()
			.Property(item => item.Price)
			.HasPrecision(9, 2);
		modelBuilder.Entity<OrderItem>()
			.Property(item => item.Subtotal)
			.HasPrecision(9, 2);
		modelBuilder.Entity<OrderItem>()
			.Property(item => item.Note)
			.HasMaxLength(30);

		base.OnModelCreating(modelBuilder);
	}
}
