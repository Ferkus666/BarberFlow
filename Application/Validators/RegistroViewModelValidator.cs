using BarberFlow.ViewModels.Conta;
using FluentValidation;

namespace BarberFlow.Application.Validators;

public sealed class RegistroViewModelValidator : AbstractValidator<RegistroViewModel>
{
    public RegistroViewModelValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().Length(3, 100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmarSenha).Equal(x => x.Senha).WithMessage("As senhas não conferem.");
    }
}
