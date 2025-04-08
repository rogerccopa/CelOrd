using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ClientDbContext : DbContext
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
		modelBuilder.Entity<User>().Property(entity => entity.CreatedAt).HasDefaultValueSql("getdate()");
		modelBuilder.Entity<Order>().Property(entity => entity.CreatedAt).HasDefaultValueSql("getdate()");

		modelBuilder.Ignore<UserClaim>();

		modelBuilder.Entity<Order>().HasMany(ord => ord.OrderItems).WithOne()
			.HasForeignKey(orditm => orditm.OrderId);

		base.OnModelCreating(modelBuilder);
	}
}
