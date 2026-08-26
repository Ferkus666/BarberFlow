using System.ComponentModel.DataAnnotations;

namespace BarberFlow.ViewModels.Conta;

public class RegistroViewModel
{
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter ao menos 6 caracteres.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme sua senha.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
    [Display(Name = "Confirmar senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
