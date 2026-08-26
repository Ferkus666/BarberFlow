using System.ComponentModel.DataAnnotations;
namespace BarberFlow.Application.DTOs.Requests;
public sealed class ClienteRequest
{
    [Required, StringLength(100)] public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = string.Empty;
    [Phone, StringLength(20)] public string? Telefone { get; set; }
}
