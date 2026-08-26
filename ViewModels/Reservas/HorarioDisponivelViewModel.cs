namespace BarberFlow.ViewModels.Reservas;

public class HorarioDisponivelViewModel
{
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
    public string Rotulo => $"{Inicio:HH:mm} às {Fim:HH:mm}";
}
