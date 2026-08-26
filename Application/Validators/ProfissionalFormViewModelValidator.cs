using BarberFlow.ViewModels.Profissionais;
using FluentValidation;

namespace BarberFlow.Application.Validators;

public sealed class ProfissionalFormViewModelValidator : AbstractValidator<ProfissionalFormViewModel>
{
    public ProfissionalFormViewModelValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().Length(3, 100);
        RuleFor(x => x.Biografia).MaximumLength(500);
        RuleFor(x => x.Especialidade).MaximumLength(120);
        RuleFor(x => x.AnosExperiencia).InclusiveBetween(0, 60).When(x => x.AnosExperiencia.HasValue);
        RuleFor(x => x.Instagram).MaximumLength(120);
        RuleFor(x => x.Foto)
            .Must(f => f is null || f.Length <= 3 * 1024 * 1024)
            .WithMessage("A foto deve ter no máximo 3 MB.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Senha).MinimumLength(6).When(x => !string.IsNullOrWhiteSpace(x.Senha));
        RuleFor(x => x.ServicosSelecionados)
            .NotEmpty().WithMessage("Selecione pelo menos um serviço realizado pelo profissional.");
    }
}
