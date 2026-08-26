using BarberFlow.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Controllers;

public class ServicosController : Controller
{
    private readonly IServicoService _service;

    public ServicosController(IServicoService service) => _service = service;

    public async Task<IActionResult> Index() => View(await _service.ListarAtivosAsync());

    public async Task<IActionResult> Detalhes(int id)
    {
        var servico = await _service.BuscarPorIdAsync(id);
        return servico is null ? NotFound() : View(servico);
    }
}
