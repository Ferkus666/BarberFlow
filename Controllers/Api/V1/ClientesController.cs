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
[ApiController,Route("api/v1/clientes"),Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme,Roles=IdentitySeed.AdministradorRole)]
public sealed class ClientesController:ControllerBase
{
 private readonly BarberFlowDbContext _context;private readonly IMapper _mapper;public ClientesController(BarberFlowDbContext c,IMapper m){_context=c;_mapper=m;}
 [HttpGet]public async Task<IActionResult> Get([FromQuery]string? busca,[FromQuery]int page=1,[FromQuery]int pageSize=10){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=_context.Clientes.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(busca))q=q.Where(c=>c.Nome.Contains(busca)||c.Email.Contains(busca));var total=await q.CountAsync();var items=await q.OrderBy(c=>c.Nome).Skip((page-1)*pageSize).Take(pageSize).ProjectTo<ClienteDto>(_mapper.ConfigurationProvider).ToListAsync();return Ok(ApiResponse<PagedResponse<ClienteDto>>.Ok(new(){Items=items,Page=page,PageSize=pageSize,TotalItems=total}));}
 [HttpGet("{id:int}")]public async Task<IActionResult> GetById(int id){var x=await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c=>c.Id==id);return x is null?NotFound(ApiResponse<object>.Fail("Cliente não encontrado.")):Ok(ApiResponse<ClienteDto>.Ok(_mapper.Map<ClienteDto>(x)));}
 [HttpPost]public async Task<IActionResult> Post(ClienteRequest r){if(await _context.Clientes.AnyAsync(c=>c.Email==r.Email.Trim()))return Conflict(ApiResponse<object>.Fail("Já existe cliente com este e-mail."));var x=new Cliente{Nome=r.Nome.Trim(),Email=r.Email.Trim(),Telefone=r.Telefone?.Trim()};_context.Add(x);await _context.SaveChangesAsync();return CreatedAtAction(nameof(GetById),new{id=x.Id},ApiResponse<ClienteDto>.Ok(_mapper.Map<ClienteDto>(x),"Cliente criado."));}
 [HttpPut("{id:int}")]public async Task<IActionResult> Put(int id,ClienteRequest r){var x=await _context.Clientes.FindAsync(id);if(x is null)return NotFound(ApiResponse<object>.Fail("Cliente não encontrado."));x.Nome=r.Nome.Trim();x.Email=r.Email.Trim();x.Telefone=r.Telefone?.Trim();await _context.SaveChangesAsync();return Ok(ApiResponse<ClienteDto>.Ok(_mapper.Map<ClienteDto>(x),"Cliente atualizado."));}
}
