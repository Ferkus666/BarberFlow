using BarberFlow.Enums;
using BarberFlow.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberFlow.ViewModels.Relatorios;

public class RelatorioReservasViewModel
{
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public int? ProfissionalId { get; set; }
    public int? ServicoId { get; set; }
    public StatusReserva? Status { get; set; }
    public List<SelectListItem> Profissionais { get; set; } = [];
    public List<SelectListItem> Servicos { get; set; } = [];
    public List<Reserva> Reservas { get; set; } = [];
    public int TotalRegistros => Reservas.Count;
    public decimal ValorTotal => Reservas.Where(r => r.Status == StatusReserva.Concluida).Sum(r => r.Servico.Preco);
    public decimal ValorPrevisto => Reservas.Where(r => r.Status is StatusReserva.Pendente or StatusReserva.Confirmada).Sum(r => r.Servico.Preco);
}
