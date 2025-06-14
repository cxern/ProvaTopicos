using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Data;

namespace ReceitasApi.Rotas;

[ApiController]
[Route("api/receitas")]
public class ROTA_DELETE : ControllerBase
{
    private readonly AppDbContext _context;

    public ROTA_DELETE(AppDbContext context)
    {
        _context = context;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReceitaAsync(int id)
    {
        try
        {
            var receita = await _context.Receitas
                .Include(r => r.Alergias) 
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();
            
            _context.Alergias.RemoveRange(receita.Alergias);
            _context.Receitas.Remove(receita);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Erro ao remover receita: {dbEx.Message}");
            return StatusCode(500, "Erro ao remover receita do banco de dados.");
        }
        catch (InvalidOperationException invalidOpEx)
        {
            Console.WriteLine($"Operação inválida: {invalidOpEx.Message}");
            return BadRequest("Operação inválida ao tentar deletar a receita.");
        }
    }
}