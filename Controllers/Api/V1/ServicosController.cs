using AutoMapper;
using AutoMapper.QueryableExtensions;
using BarberFlow.Application.API;
using BarberFlow.Application.DTOs;
using BarberFlow.Application.DTOs.Requests;
using BarberFlow.Data;
using BarberFlow.Data.Seed;
using BarberFlow.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Controllers.Api.V1;
[ApiController, Route("api/v1/servicos")]
public sealed class ServicosController : ControllerBase
{
    private readonly BarberFlowDbContext _context; private readonly IMapper _mapper; private readonly ILogger<ServicosController> _logger;
    public ServicosController(BarberFlowDbContext context,IMapper mapper,ILogger<ServicosController> logger){_context=context;_mapper=mapper;_logger=logger;}

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<ServicoDto>>>> Get([FromQuery]string? nome,[FromQuery]bool? ativo,[FromQuery]int page=1,[FromQuery]int pageSize=10)
    {
        page=Math.Max(page,1); pageSize=Math.Clamp(pageSize,1,100);
        var q=_context.Servicos.AsNoTracking().AsQueryable();
        if(!string.IsNullOrWhiteSpace(nome)) q=q.Where(x=>x.Nome.Contains(nome));
        if(ativo.HasValue) q=q.Where(x=>x.Ativo==ativo);
        var total=await q.CountAsync();
        var items=await q.OrderBy(x=>x.Nome).Skip((page-1)*pageSize).Take(pageSize).ProjectTo<ServicoDto>(_mapper.ConfigurationProvider).ToListAsync();
        return Ok(ApiResponse<PagedResponse<ServicoDto>>.Ok(new(){Items=items,Page=page,PageSize=pageSize,TotalItems=total}));
    }
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){var x=await _context.Servicos.AsNoTracking().FirstOrDefaultAsync(s=>s.Id==id);return x is null?NotFound(ApiResponse<object>.Fail("Serviço não encontrado.")):Ok(ApiResponse<ServicoDto>.Ok(_mapper.Map<ServicoDto>(x)));}
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles=IdentitySeed.AdministradorRole),HttpPost]
    public async Task<IActionResult> Post(ServicoRequest request){var entity=new Servico{Nome=request.Nome.Trim(),Descricao=request.Descricao?.Trim(),DuracaoMinutos=request.DuracaoMinutos,Preco=request.Preco,Ativo=request.Ativo};_context.Add(entity);await _context.SaveChangesAsync();_logger.LogInformation("Serviço {ServicoId} criado via API",entity.Id);return CreatedAtAction(nameof(GetById),new{id=entity.Id},ApiResponse<ServicoDto>.Ok(_mapper.Map<ServicoDto>(entity),"Serviço criado."));}
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles=IdentitySeed.AdministradorRole),HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id,ServicoRequest request){var x=await _context.Servicos.FindAsync(id);if(x is null)return NotFound(ApiResponse<object>.Fail("Serviço não encontrado."));x.Nome=request.Nome.Trim();x.Descricao=request.Descricao?.Trim();x.DuracaoMinutos=request.DuracaoMinutos;x.Preco=request.Preco;x.Ativo=request.Ativo;await _context.SaveChangesAsync();return Ok(ApiResponse<ServicoDto>.Ok(_mapper.Map<ServicoDto>(x),"Serviço atualizado."));}
    [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles=IdentitySeed.AdministradorRole),HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id){var x=await _context.Servicos.FindAsync(id);if(x is null)return NotFound(ApiResponse<object>.Fail("Serviço não encontrado."));x.Ativo=false;await _context.SaveChangesAsync();return Ok(ApiResponse<object>.Ok(new{id},"Serviço desativado."));}
}
