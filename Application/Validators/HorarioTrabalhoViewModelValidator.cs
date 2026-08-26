using BarberFlow.ViewModels.Profissionais;
using FluentValidation;

namespace BarberFlow.Application.Validators;

public sealed class HorarioTrabalhoViewModelValidator : AbstractValidator<HorarioTrabalhoViewModel>
{
    public HorarioTrabalhoViewModelValidator()
    {
        RuleFor(x => x.HoraFim)
            .GreaterThan(x => x.HoraInicio)
            .WithMessage("O horário final deve ser posterior ao inicial.");
        RuleFor(x => x.HoraInicio).Must(h => h >= TimeSpan.Zero && h < TimeSpan.FromDays(1));
        RuleFor(x => x.HoraFim).Must(h => h > TimeSpan.Zero && h <= TimeSpan.FromDays(1));
    }
}
