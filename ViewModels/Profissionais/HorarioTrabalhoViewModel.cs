using System.ComponentModel.DataAnnotations;

namespace BarberFlow.ViewModels.Profissionais;

public class HorarioTrabalhoViewModel : IValidatableObject
{
    public int ProfissionalId { get; set; }
    public string ProfissionalNome { get; set; } = string.Empty;

    [Display(Name = "Dia da semana")]
    public DayOfWeek DiaSemana { get; set; }

    [Required(ErrorMessage = "Informe o horário inicial.")]
    [Display(Name = "Início")]
    public TimeSpan HoraInicio { get; set; } = new(9, 0, 0);

    [Required(ErrorMessage = "Informe o horário final.")]
    [Display(Name = "Fim")]
    public TimeSpan HoraFim { get; set; } = new(18, 0, 0);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HoraFim <= HoraInicio)
            yield return new ValidationResult("O horário final deve ser posterior ao inicial.", new[] { nameof(HoraFim) });
    }
}
