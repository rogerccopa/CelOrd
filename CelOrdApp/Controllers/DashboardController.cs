using CelOrdApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace CelOrdApp.Controllers;

public class DashboardController : Controller
{
    private readonly ClientDbContext _clientDbCtx;

    public DashboardController(ClientDbContext clientDbContext)
    {
        _clientDbCtx = clientDbContext;
    }

    public IActionResult Index()
    {
        // Implement your logic to fetch dashboard data here
        var dashboardData = new
        {
            // Example data
            TotalOrders = 100,
            TotalSales = 5000,
            TotalCustomers = 200
        };

        return View(dashboardData);
    }
}
