using BarberFlow.Enums;
using BarberFlow.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberFlow.ViewModels.Reservas;

public class ReservaFiltroViewModel
{
    public DateTime? Data { get; set; }
    public int? ProfissionalId { get; set; }
    public int? ServicoId { get; set; }
    public StatusReserva? Status { get; set; }
    public List<SelectListItem> Profissionais { get; set; } = new();
    public List<SelectListItem> Servicos { get; set; } = new();
    public List<Reserva> Reservas { get; set; } = new();
}
