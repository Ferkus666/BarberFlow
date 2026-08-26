using BarberFlow.ViewModels.Dashboard;

namespace BarberFlow.Interfaces;

public interface IDashboardService
{
    Task<DashboardAdminViewModel> ObterDashboardAsync();
}
