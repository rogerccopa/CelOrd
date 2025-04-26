namespace Domain
{
	public enum UserType : byte { Unknown, Attendant, Cook, Cashier, Admin }
	public enum OrderState : byte { New, InProcess, Ready, Payed, Canceled }
}
