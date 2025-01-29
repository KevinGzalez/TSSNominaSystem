using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TSSNominaSystem.Models
{
    public class Nomina
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmpleadoId")]
        public int EmpleadoId { get; set; }

        public Empleado? Empleado { get; set; } // Nullable para evitar errores de inicialización

        [Required]
        public decimal Sueldo { get; set; }

        [Required]
        public decimal Deducciones { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }
    }
}
