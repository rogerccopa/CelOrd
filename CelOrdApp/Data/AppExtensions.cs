using static Domain.EntityTypes;

namespace CelOrdApp.Data;

public static class AppExtensions
{
	public static string ToSpanish(this Area userType)
	{
		return userType switch
		{
			Area.Admin => "Admin",
			Area.Cashier => "Caja",
			Area.Kitchen => "Cocina",
			Area.Service => "Servicio",
			_ => throw new ArgumentOutOfRangeException(nameof(userType), userType, null)
		};
	}
}
