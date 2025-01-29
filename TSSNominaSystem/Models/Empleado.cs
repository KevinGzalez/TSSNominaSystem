namespace TSSNominaSystem.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public decimal Salario { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Activo { get; set; }
    }
}
