using AutoMapper;
using BarberFlow.Application.API;
using BarberFlow.Application.DTOs;
using BarberFlow.Application.DTOs.Requests;
using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Enums;
using BarberFlow.Interfaces;
using BarberFlow.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BarberFlow.Controllers.Api.V1;
[ApiController,Route("api/v1/reservas"),Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
public sealed class ReservasController:ControllerBase
{
 private readonly BarberFlowDbContext _context;private readonly IMapper _mapper;private readonly IDisponibilidadeService _disponibilidade;private readonly ILogger<ReservasController> _logger;
 public ReservasController(BarberFlowDbContext c,IMapper m,IDisponibilidadeService d,ILogger<ReservasController> l){_context=c;_mapper=m;_disponibilidade=d;_logger=l;}
 [HttpGet]public async Task<IActionResult> Get([FromQuery]DateTime? dataInicio,[FromQuery]DateTime? dataFim,[FromQuery]StatusReserva? status,[FromQuery]int? profissionalId,[FromQuery]int page=1,[FromQuery]int pageSize=10){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=_context.Reservas.AsNoTracking().Include(r=>r.Cliente).Include(r=>r.Profissional).Include(r=>r.Servico).AsQueryable();if(User.IsInRole(IdentitySeed.ClienteRole))q=q.Where(r=>r.Cliente.Email==User.Identity!.Name);else if(User.IsInRole(IdentitySeed.ProfissionalRole))q=q.Where(r=>r.Profissional.Usuario!=null&&r.Profissional.Usuario.Email==User.Identity!.Name);if(dataInicio.HasValue)q=q.Where(r=>r.DataHoraInicio>=dataInicio.Value);if(dataFim.HasValue)q=q.Where(r=>r.DataHoraInicio<dataFim.Value.Date.AddDays(1));if(status.HasValue)q=q.Where(r=>r.Status==status);if(profissionalId.HasValue)q=q.Where(r=>r.ProfissionalId==profissionalId);var total=await q.CountAsync();var entities=await q.OrderByDescending(r=>r.DataHoraInicio).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();return Ok(ApiResponse<PagedResponse<ReservaDto>>.Ok(new(){Items=_mapper.Map<List<ReservaDto>>(entities),Page=page,PageSize=pageSize,TotalItems=total}));}
 [HttpGet("{id:int}")]public async Task<IActionResult> GetById(int id){var x=await _context.Reservas.AsNoTracking().Include(r=>r.Cliente).Include(r=>r.Profissional).Include(r=>r.Servico).FirstOrDefaultAsync(r=>r.Id==id);return x is null?NotFound(ApiResponse<object>.Fail("Reserva não encontrada.")):Ok(ApiResponse<ReservaDto>.Ok(_mapper.Map<ReservaDto>(x)));}
 [HttpPost]public async Task<IActionResult> Post(ReservaApiRequest r){var cliente=await _context.Clientes.FindAsync(r.ClienteId);if(User.IsInRole(IdentitySeed.ClienteRole)&&cliente?.Email!=User.Identity?.Name)return Forbid();var servico=await _context.Servicos.FindAsync(r.ServicoId);if(cliente is null||servico is null)return BadRequest(ApiResponse<object>.Fail("Cliente ou serviço inválido."));if(!await _disponibilidade.HorarioEstaDisponivelAsync(r.ProfissionalId,r.ServicoId,r.DataHoraInicio))return Conflict(ApiResponse<object>.Fail("Horário indisponível."));var x=new Reserva{ClienteId=r.ClienteId,ProfissionalId=r.ProfissionalId,ServicoId=r.ServicoId,DataHoraInicio=r.DataHoraInicio,DataHoraFim=r.DataHoraInicio.AddMinutes(servico.DuracaoMinutos),Status=StatusReserva.Confirmada};_context.Add(x);await _context.SaveChangesAsync();x.Cliente=cliente;x.Servico=servico;x.Profissional=(await _context.Profissionais.FindAsync(r.ProfissionalId))!;_logger.LogInformation("Reserva {ReservaId} criada via API",x.Id);return CreatedAtAction(nameof(GetById),new{id=x.Id},ApiResponse<ReservaDto>.Ok(_mapper.Map<ReservaDto>(x),"Reserva criada."));}
 [Authorize(Roles=IdentitySeed.AdministradorRole+","+IdentitySeed.ProfissionalRole),HttpPatch("{id:int}/status")]public async Task<IActionResult> Status(int id,AtualizarStatusReservaRequest r){var x=await _context.Reservas.Include(x=>x.Cliente).Include(x=>x.Profissional).Include(x=>x.Servico).FirstOrDefaultAsync(x=>x.Id==id);if(x is null)return NotFound(ApiResponse<object>.Fail("Reserva não encontrada."));x.Status=r.Status;await _context.SaveChangesAsync();return Ok(ApiResponse<ReservaDto>.Ok(_mapper.Map<ReservaDto>(x),"Status atualizado."));}
 [HttpDelete("{id:int}")]public async Task<IActionResult> Cancelar(int id){var x=await _context.Reservas.Include(x=>x.Cliente).Include(x=>x.Profissional).Include(x=>x.Servico).FirstOrDefaultAsync(x=>x.Id==id);if(x is not null&&User.IsInRole(IdentitySeed.ClienteRole)&&x.Cliente.Email!=User.Identity?.Name)return Forbid();if(x is null)return NotFound(ApiResponse<object>.Fail("Reserva não encontrada."));if(x.Status==StatusReserva.Concluida)return BadRequest(ApiResponse<object>.Fail("Reserva concluída não pode ser cancelada."));x.Status=StatusReserva.Cancelada;await _context.SaveChangesAsync();return Ok(ApiResponse<ReservaDto>.Ok(_mapper.Map<ReservaDto>(x),"Reserva cancelada."));}
}
