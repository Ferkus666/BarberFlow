using AutoMapper;
using BarberFlow.Application.API;
using BarberFlow.Application.DTOs;
using BarberFlow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BarberFlow.Controllers.Api.V1;
[ApiController,Route("api/v1/profissionais")]
public sealed class ProfissionaisController:ControllerBase
{
 private readonly BarberFlowDbContext _context;private readonly IMapper _mapper;public ProfissionaisController(BarberFlowDbContext c,IMapper m){_context=c;_mapper=m;}
 [HttpGet] public async Task<IActionResult> Get([FromQuery]bool? ativo,[FromQuery]int? servicoId,[FromQuery]int page=1,[FromQuery]int pageSize=10){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=_context.Profissionais.AsNoTracking().Include(p=>p.ProfissionalServicos).ThenInclude(ps=>ps.Servico).AsQueryable();if(ativo.HasValue)q=q.Where(p=>p.Ativo==ativo);if(servicoId.HasValue)q=q.Where(p=>p.ProfissionalServicos.Any(ps=>ps.ServicoId==servicoId));var total=await q.CountAsync();var entities=await q.OrderBy(p=>p.Nome).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();var items=_mapper.Map<List<ProfissionalDto>>(entities);return Ok(ApiResponse<PagedResponse<ProfissionalDto>>.Ok(new(){Items=items,Page=page,PageSize=pageSize,TotalItems=total}));}
 [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var x=await _context.Profissionais.AsNoTracking().Include(p=>p.ProfissionalServicos).ThenInclude(ps=>ps.Servico).FirstOrDefaultAsync(p=>p.Id==id);return x is null?NotFound(ApiResponse<object>.Fail("Profissional não encontrado.")):Ok(ApiResponse<ProfissionalDto>.Ok(_mapper.Map<ProfissionalDto>(x)));}
}
