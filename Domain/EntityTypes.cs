namespace Domain;

public class EntityTypes
{
	public enum UserType : byte { Unknown, Attendant, Cook, Cashier, Admin }
	public enum OrderState : byte { New, InProcess, Ready, Payed, Canceled }
}
