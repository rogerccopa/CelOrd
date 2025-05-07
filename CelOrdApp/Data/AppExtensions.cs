using static Domain.EntityTypes;

namespace CelOrdApp.Data;

public static class AppExtensions
{
	public static string ToSpanish(this UserType userType)
	{
		return userType switch
		{
			UserType.Unknown => "",
			UserType.Admin => "Admin",
			UserType.Cashier => "Caja",
			UserType.Cook => "Cocina",
			UserType.Attendant => "Servicio",
			_ => throw new ArgumentOutOfRangeException(nameof(userType), userType, null)
		};
	}
}
