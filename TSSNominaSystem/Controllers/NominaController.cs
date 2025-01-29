using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TSSNominaSystem.Data;
using TSSNominaSystem.Models;

namespace TSSNominaSystem.Controllers
{
    public class NominaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NominaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listado de nóminas
        public async Task<IActionResult> Index()
        {
            var nominas = await _context.Nominas
                .Include(n => n.Empleado)
                .ToListAsync();
            return View(nominas);  // Asegurar que View() está llamando a la vista correcta
        }


        // Formulario de creación
        public IActionResult Create()
        {
            var empleados = _context.Empleados
                                    .Where(e => e.Activo)
                                    .Select(e => new { e.Id, e.Nombre })
                                    .ToList();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre");
            return View();
        }

        // Guardar nómina en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpleadoId,Sueldo,Deducciones,FechaPago")] Nomina nomina)
        {
            if (nomina.EmpleadoId == 0)
            {
                ModelState.AddModelError("EmpleadoId", "Debe seleccionar un empleado.");
            }

            if (!ModelState.IsValid)
            {
                // Mostrar errores de validación en consola
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Error en el modelo: " + error.ErrorMessage);
                }

                var empleados = _context.Empleados
                                        .Where(e => e.Activo)
                                        .Select(e => new { e.Id, e.Nombre })
                                        .ToList();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", nomina.EmpleadoId);
                return View(nomina);
            }

            try
            {
                _context.Nominas.Add(nomina);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Nómina guardada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar: {ex.Message}");
                ModelState.AddModelError("", "Error al guardar la nómina. Intente nuevamente.");

                var empleados = _context.Empleados
                                        .Where(e => e.Activo)
                                        .Select(e => new { e.Id, e.Nombre })
                                        .ToList();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", nomina.EmpleadoId);
                return View(nomina);
            }

            return RedirectToAction(nameof(Index));
        }

        // Método para mostrar el formulario de edición de nómina
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null)
            {
                return NotFound();
            }

            ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => e.Activo), "Id", "Nombre", nomina.EmpleadoId);
            return View(nomina);
        }

        // Método para recibir el formulario de edición de nómina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpleadoId,Sueldo,Deducciones,FechaPago")] Nomina nomina)
        {
            if (id != nomina.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => e.Activo), "Id", "Nombre", nomina.EmpleadoId);
                return View(nomina);
            }

            try
            {
                _context.Update(nomina);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Nómina actualizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar: {ex.Message}");
                ModelState.AddModelError("", "Error al actualizar la nómina. Intente nuevamente.");
                ViewBag.Empleados = new SelectList(_context.Empleados.Where(e => e.Activo), "Id", "Nombre", nomina.EmpleadoId);
                return View(nomina);
            }

            return RedirectToAction(nameof(Index));
        }

        // Método para mostrar el formulario de confirmación de eliminación de nómina
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas
                .Include(n => n.Empleado)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nomina == null)
            {
                return NotFound();
            }

            return View(nomina);
        }

        // Método para recibir la confirmación y eliminar la nómina
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null)
            {
                return NotFound();
            }

            try
            {
                _context.Nominas.Remove(nomina);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Nómina eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar: {ex.Message}");
                ModelState.AddModelError("", "Error al eliminar la nómina. Intente nuevamente.");
                return View(nomina);
            }

            return RedirectToAction(nameof(Index));
        }


        // Exportar a TSS en formato Excel
        [HttpGet]
        public async Task<IActionResult> ExportarTSS()
        {
            var nominas = await _context.Nominas
                .Include(n => n.Empleado)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Nominas");
                worksheet.Cell(1, 1).Value = "Cédula";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Sueldo";
                worksheet.Cell(1, 4).Value = "Deducciones";
                worksheet.Cell(1, 5).Value = "Fecha de Pago";

                int row = 2;
                foreach (var nomina in nominas)
                {
                    worksheet.Cell(row, 1).Value = nomina.Empleado?.Cedula;
                    worksheet.Cell(row, 2).Value = nomina.Empleado?.Nombre;
                    worksheet.Cell(row, 3).Value = nomina.Sueldo;
                    worksheet.Cell(row, 4).Value = nomina.Deducciones;
                    worksheet.Cell(row, 5).Value = nomina.FechaPago.ToShortDateString();
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Nominas_TSS.xlsx");
                }
            }
        }

    }
}