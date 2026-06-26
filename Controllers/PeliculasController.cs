using Microsoft.AspNetCore.Mvc;
using ReelTalk.Api.Data;
using ReelTalk.Api.Modelos;

namespace ReelTalk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {
        private readonly ReelTalkDbContext _context;

        //inyectamos nuestro DBContext a traves del constructor

        public PeliculasController(ReelTalkDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CrearPelicula([FromBody] Pelicula nuevaPelicula)
        {
            // 1. Validar que el objeto no venga vacio 
            if (nuevaPelicula == null)
            {
                return BadRequest("Los datos de la pelicula no son válidos.");
            }

            // 2. Preparar el objeto de Entity Framework Core
            _context.Peliculas.Add(nuevaPelicula);

            // 3. Impactar y guardar los cambios reales en el SQL Server
            _context.SaveChanges();

            // 4. Retornar una respuesta exitosa (HTTP 200 OK)
            return Ok(new { mensaje = "Película creada con éxito", datos = nuevaPelicula });
        }

        [HttpGet]
        public IActionResult ObtenerPeliculas() 
        {
            // 1. Ir a la base de datos a traves de EF Core y traer la lista completa
            var listaPelicula = _context.Peliculas.ToList();

            // 2. Devolver la lista con un codigo 200 OK
            return Ok(listaPelicula);   

        }

    }

}
