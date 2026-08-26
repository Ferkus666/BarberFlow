namespace BarberFlow.ViewModels;

public sealed class ErroViewModel
{
    public int StatusCode { get; set; }
    public string TraceId { get; set; } = string.Empty;
}
