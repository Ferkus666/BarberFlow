using System.ComponentModel.DataAnnotations;
namespace BarberFlow.Application.DTOs.Requests;
public sealed class LoginApiRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Senha { get; set; } = string.Empty;
}
