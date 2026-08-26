using BarberFlow.Data.Seed;
using BarberFlow.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Controllers;

[Authorize(Roles = IdentitySeed.AdministradorRole)]
public class AdminController : Controller
{
    private readonly IDashboardService _dashboardService;
    public AdminController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    public async Task<IActionResult> Index() => View(await _dashboardService.ObterDashboardAsync());
}
