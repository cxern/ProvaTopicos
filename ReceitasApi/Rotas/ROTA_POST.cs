using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Data;
using ReceitasApi.Model;

namespace ReceitasApi.Rotas;

[ApiController]
[Route("api/receitas")]
public class ROTA_POST : ControllerBase
{
    private readonly AppDbContext _context;

    public ROTA_POST(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarReceitaAsync([FromBody] Receita novaReceita)
    {
        if (novaReceita == null || novaReceita.Alergias == null)
            return BadRequest("Dados Inválidos");

        if (novaReceita.Alergias.Any(alergia => alergia == null))
            return BadRequest("Uma ou mais alergias são inválidas.");

        try
        {
            await _context.Receitas.AddAsync(novaReceita);
            await _context.SaveChangesAsync();

            foreach (var alergia in novaReceita.Alergias)
            {
                alergia.ReceitaId = novaReceita.Id;
            }

            await _context.SaveChangesAsync();

            return CreatedAtRoute(
                routeName: "GetReceitaPorId", 
                routeValues: new { id = novaReceita.Id }, 
                value: novaReceita);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Erro ao criar receita: {dbEx.Message}");
            return StatusCode(500, "Erro ao criar receita no banco de dados.");
        }
    }
}