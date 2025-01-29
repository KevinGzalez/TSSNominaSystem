namespace TSSNominaSystem.Models
{
    public class Novedad
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public string TipoNovedad { get; set; }
        public DateTime Fecha { get; set; }

        public Empleado Empleado { get; set; }
    }
}
