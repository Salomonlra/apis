using Microsoft.EntityFrameworkCore;
using RTK_API_Active.Models;

namespace RTK_API_Active.DBcontex
{
    public class EquipmentControlContext : DbContext
    {
        public EquipmentControlContext(DbContextOptions<EquipmentControlContext> options) : base(options)
        {
        }

        public DbSet<Vectro_Gps> Vectro_Gps { get; set; }
       
    }
}