using BarberFlow.ViewModels.Reservas;
using FluentValidation;

namespace BarberFlow.Application.Validators;

public sealed class NovaReservaViewModelValidator : AbstractValidator<NovaReservaViewModel>
{
    public NovaReservaViewModelValidator()
    {
        RuleFor(x => x.ServicoId).GreaterThan(0).WithMessage("Selecione um serviço.");
        RuleFor(x => x.ProfissionalId).GreaterThan(0).WithMessage("Selecione um profissional.");
        RuleFor(x => x.Data).GreaterThanOrEqualTo(DateTime.Today).WithMessage("Escolha uma data de hoje em diante.");
        RuleFor(x => x.Horario).NotEmpty().WithMessage("Selecione um horário.");
    }
}
