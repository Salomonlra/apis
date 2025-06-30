using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RTK_API_Active.DBcontex;
using RTK_API_Active.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RTK_API_Active.Controllers
{
    [Route("api/active-rtgs")]
    [ApiController]
    public class VectroGpsController : ControllerBase
    {
        private readonly EquipmentControlContext _context;

        public VectroGpsController(EquipmentControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestRecords()
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(-6); // Hora local (cdmx)
                var last20Minutes = now.AddMinutes(-20);

                // Obtener datos filtrados
                var vectros = await _context.Vectro_Gps
                    .AsNoTracking()
                    .Where(v =>
                        v.EQUIPMENT.StartsWith("RT-") &&
                        v.DATE == now.Date &&
                        (
                            v.IN_1 == "1" || // Ignición encendida
                            v.LOG_DATE >= last20Minutes // Movimiento reciente
                        )

                    )
                    .ToListAsync();

                // Agrupar por equipo y obtener el último registro
                var grouped = vectros
                    .GroupBy(v => v.EQUIPMENT)
                    .Select(g => g.OrderByDescending(v => v.LOG_DATE).FirstOrDefault())
                    .Select(v => new
                    {
                        Name = v.EQUIPMENT,
                        DATE = v.DATE.ToString("yyyy-MM-dd"),
                        Time = v.TIME.ToString(@"hh\:mm\:ss"),
                        v.LAT,
                        v.LON,
                        IN = v.IN_1,
                        TriggerType = v.IN_1 == "1" ? "Ignition" : "Movement"
                    })
                    .ToList();

                if (!grouped.Any())
                {
                    return BadRequest(new
                    {
                        MESSAGE = new
                        {
                            Code = 400,
                            Status = "Failed",
                            Description = "No active RTGs found."
                        },
                        rtgs_activas = new object[] { },
                        SUMMARY = new
                        {
                            Total = 0,
                            ByType = new
                            {
                                Ignition = 0,
                                Movement = 0
                            }
                        }
                    });
                }

                // Conteo por tipo
                var ignitionCount = grouped.Count(r => r.TriggerType == "Ignition");
                var movementCount = grouped.Count(r => r.TriggerType == "Movement");

                return Ok(new
                {
                    MESSAGE = new
                    {
                        Code = 200,
                        Status = "Success",
                        Description = "Active RTGs found successfully."
                    },
                    rtgs_activas = grouped,
                    SUMMARY = new
                    {
                        Total = grouped.Count,
                        ByType = new
                        {
                            Ignition = ignitionCount,
                            Movement = movementCount
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    MESSAGE = new
                    {
                        Code = 400,
                        Status = "Error",
                        Description = $"An error occurred: {ex.Message}"
                    },
                    rtgs_activas = new object[] { },
                    SUMMARY = new
                    {
                        Total = 0,
                        ByType = new
                        {
                            Ignition = 0,
                            Movement = 0
                        }
                    }
                });
            }
        }
    }
}
