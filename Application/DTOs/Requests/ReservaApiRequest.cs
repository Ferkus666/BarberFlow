using System.ComponentModel.DataAnnotations;
namespace BarberFlow.Application.DTOs.Requests;
public sealed class ReservaApiRequest
{
    [Range(1,int.MaxValue)] public int ClienteId { get; set; }
    [Range(1,int.MaxValue)] public int ProfissionalId { get; set; }
    [Range(1,int.MaxValue)] public int ServicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
}
