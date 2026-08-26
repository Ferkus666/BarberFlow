using BarberFlow.Models;
using FluentValidation;

namespace BarberFlow.Application.Validators;

public sealed class ServicoValidator : AbstractValidator<Servico>
{
    public ServicoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().Length(2, 100);
        RuleFor(x => x.Descricao).MaximumLength(500);
        RuleFor(x => x.DuracaoMinutos).InclusiveBetween(15, 480)
            .Must(x => x % 15 == 0).WithMessage("A duração deve ser informada em intervalos de 15 minutos.");
        RuleFor(x => x.Preco).GreaterThan(0).LessThanOrEqualTo(99999);
    }
}
