namespace Domain;

public class EntityTypes
{
	public enum Area : byte { Service, Kitchen, Cashier, Admin }
	public enum OrderState : byte { New, InProcess, Ready, Payed, Canceled }
}
