using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Data;

namespace ReceitasApi.Rotas;

[ApiController]
[Route("api/receitas")]
public class ROTA_GET : ControllerBase
{
    private readonly AppDbContext _context;

    public ROTA_GET(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var receitas = await _context.Receitas
            .Include(r => r.Alergias)
            .AsNoTracking()
            .ToListAsync();
        return Ok(receitas);
    }

    [HttpGet("{id}", Name = "GetReceitaPorId")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var receita = await _context.Receitas
            .Include(r => r.Alergias)
            .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            return Ok(receita);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Erro ao atualizar o banco de dados: {dbEx.Message}");
            return StatusCode(500, "Erro ao atualizar o banco de dados.");
        }
    }

    [HttpGet("alergias")]
    public async Task<IActionResult> GetAlergias()
    {
        var alergias = await _context.Alergias
            .Select(a => a.Nome)
            .Distinct()
            .OrderBy(nome => nome)
            .ToListAsync();

        return Ok(alergias);
    }
}