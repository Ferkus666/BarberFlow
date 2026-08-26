using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BarberFlow.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
