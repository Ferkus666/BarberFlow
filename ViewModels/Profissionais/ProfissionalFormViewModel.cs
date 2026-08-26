using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberFlow.ViewModels.Profissionais;

public class ProfissionalFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do profissional.")]
    [StringLength(100)]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Biografia profissional")]
    public string? Biografia { get; set; }

    [StringLength(120)]
    [Display(Name = "Especialidade")]
    public string? Especialidade { get; set; }

    [Range(0, 60)]
    [Display(Name = "Anos de experiência")]
    public int? AnosExperiencia { get; set; }

    [StringLength(120)]
    [Display(Name = "Instagram")]
    public string? Instagram { get; set; }

    public string? FotoUrlAtual { get; set; }

    [Display(Name = "Foto do profissional")]
    public IFormFile? Foto { get; set; }

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [Display(Name = "E-mail de acesso")]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
    [Display(Name = "Senha inicial")]
    public string? Senha { get; set; }

    [Display(Name = "Profissional ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Serviços realizados")]
    public List<int> ServicosSelecionados { get; set; } = new();
    public List<SelectListItem> ServicosDisponiveis { get; set; } = new();
}
