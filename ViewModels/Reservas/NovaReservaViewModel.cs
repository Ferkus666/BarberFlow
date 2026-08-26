using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberFlow.ViewModels.Reservas;

public class NovaReservaViewModel
{
    [Required(ErrorMessage = "Selecione um serviço.")]
    [Display(Name = "Serviço")]
    public int ServicoId { get; set; }

    [Required(ErrorMessage = "Selecione um profissional.")]
    [Display(Name = "Profissional")]
    public int ProfissionalId { get; set; }

    [Required(ErrorMessage = "Informe a data.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data")]
    public DateTime Data { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Selecione um horário.")]
    [Display(Name = "Horário")]
    public string Horario { get; set; } = string.Empty;

    public List<SelectListItem> Servicos { get; set; } = [];
    public List<SelectListItem> Profissionais { get; set; } = [];
    public IReadOnlyList<HorarioDisponivelViewModel> HorariosDisponiveis { get; set; } = [];
}
