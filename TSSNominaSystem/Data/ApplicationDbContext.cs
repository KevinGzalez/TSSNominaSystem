using Microsoft.EntityFrameworkCore;
using TSSNominaSystem.Models;

namespace TSSNominaSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Novedad> Novedades { get; set; }
        public DbSet<Nomina> Nominas { get; set; }
    }
}
