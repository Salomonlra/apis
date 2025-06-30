using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RTKlog_ping.DataContext.cs;
using RTKlog_ping.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Globalization;

[ApiController]
[Route("api/ping-results")]
public class PingResultsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PingResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // **Guardar un nuevo PingResult** (POST)
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> PostPingResult([FromBody] IEnumerable<PingResult> newPingResults)
    {
        if (newPingResults == null)
        {
            return BadRequest(new
            {
                Code = 400,
                Status = "Bad Request",
                Description = "El objeto PingResult no puede ser nulo."
            });
        }

        // Agregar el nuevo PingResult al contexto
        foreach (var pingResult in newPingResults)
        {
            await _context.PingResults.AddAsync(pingResult);
        }



        // Guardar los cambios en la base de datos
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPingResults), 1, newPingResults);
    }

    // **Obtener PingResults** (GET)
    [HttpGet]
    public async Task<IActionResult> GetPingResults([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from == default || to == default)
        {
            return BadRequest(new
            {
                MESSAGE = new
                {
                    Code = 400,
                    Status = "Bad Request",
                    Description = "Los parámetros 'from' y 'to' son obligatorios."
                }
            });
        }

        if (from > to)
        {
            return BadRequest(new
            {
                MESSAGE = new
                {
                    Code = 400,
                    Status = "Bad Request",
                    Description = "'from' no puede ser mayor que 'to'."
                }
            });
        }
        var results = await _context.PingResults
     .Where(p => p.Ping_DateTime >= from && p.Ping_DateTime <= to.AddMilliseconds(999))
     .GroupBy(p => p.Equipment)
     .Select(g => new
     {
         Equipment = g.Key,
         Historic = g.Select(p => new
         {
             p.Ping_Receive_Rate,
             Ping_DateTime = p.Ping_DateTime.ToString("hh:mm:ss tt \rdd/MM/yyyy", new CultureInfo("es-ES"))
                            .Replace("AM", "a.m.")
                            .Replace("PM", "p.m.")
         }).ToList()
     })
     .ToListAsync();

        if (!results.Any())
        {
            return NotFound(new
            {
                MESSAGE = new
                {
                    Code = 404,
                    Status = "Not Found",
                    Description = "No se encontraron resultados para el rango de fechas proporcionado."
                }
            });
        }

        return Ok(new
        {
            MESSAGE = new
            {
                Code = 200,
                Status = "OK",
                Description = "Resultados obtenidos con éxito"
            },
            Ping_Results = results
        });
    }

}

//POST /api/pingresults: Recibe datos en formato JSON y los guarda en la base de datos.
//GET/http://http://10.129.203.44:5155/api/Ping-Results?from=2020-01-01T00:00:00&to=2025-12-31T23:59:59
//dotnet ef migrations add InitialCreate
//dotnet ef database update