using System.ComponentModel.DataAnnotations;

namespace BarberFlow.ViewModels.Conta;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Display(Name = "Manter conectado")]
    public bool LembrarMe { get; set; }

    public string? ReturnUrl { get; set; }
}
