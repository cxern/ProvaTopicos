using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Data;
using ReceitasApi.Model;

namespace ReceitasApi.Rotas
{
    [ApiController]
    [Route("api/receitas")]
    public class ROTA_PUT : ControllerBase
    {
        private readonly AppDbContext _context;

        public ROTA_PUT(AppDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Receita receita)
        {
            if (id != receita.Id)
            {
                Console.WriteLine($"ID da URL: {id}, ID do corpo: {receita.Id}");
                return BadRequest("ID da URL diferente do corpo.");
            }
                

            var receitaDb = await _context.Receitas
                .Include(r => r.Alergias)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receitaDb == null)
                return NotFound();

            receitaDb.Nome = receita.Nome?.Trim();
            receitaDb.Descricao = receita.Descricao?.Trim();
            receitaDb.ModoPreparo = receita.ModoPreparo?.Trim();
            receitaDb.Valor = receita.Valor;
            receitaDb.Ingredientes = receita.Ingredientes ?? new List<string>();
            receitaDb.TempoPreparoMin = receita.TempoPreparoMin;

            receitaDb.Alergias = receita.Alergias ?? new List<Alergias>();

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}