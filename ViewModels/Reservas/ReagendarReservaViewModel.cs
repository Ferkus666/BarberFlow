using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberFlow.ViewModels.Reservas;

public class ReagendarReservaViewModel
{
    public int ReservaId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public string ProfissionalNome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova data.")]
    [DataType(DataType.Date)]
    [Display(Name = "Nova data")]
    public DateTime Data { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Selecione um novo horário.")]
    [Display(Name = "Novo horário")]
    public string Horario { get; set; } = string.Empty;

    public List<HorarioDisponivelViewModel> HorariosDisponiveis { get; set; } = new();
}
