using System.ComponentModel.DataAnnotations;

namespace BarberFlow.ViewModels.Avaliacoes;

public class AvaliacaoFormViewModel
{
    public int ReservaId { get; set; }
    public string ProfissionalNome { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "Escolha uma nota entre 1 e 5.")]
    public int Nota { get; set; } = 5;

    [StringLength(500, ErrorMessage = "O comentário deve ter no máximo 500 caracteres.")]
    public string? Comentario { get; set; }
}
