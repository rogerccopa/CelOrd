using static Domain.EntityTypes;

namespace CelOrdApp.Models;

public class Order
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public string? TableName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime FulfilledAt { get; set; }
	public DateTime PayedAt { get; set; }
	public decimal Total { get; set; }
	public OrderState State { get; set; } = OrderState.New;
	public List<OrderItem> OrderItems { get; set; } = [];
}
