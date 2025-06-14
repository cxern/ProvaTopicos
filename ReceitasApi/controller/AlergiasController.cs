using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasApi.Data;

namespace ReceitasApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlergiasController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlergiasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
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