namespace Backend.Models;

public class OrderItem
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public decimal Subtotal { get; set; }
	public string Note { get; set; } = string.Empty;
}
