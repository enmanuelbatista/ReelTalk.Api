using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReelTalk.Api.Data;
using ReelTalk.Api.Modelos;
using ReelTalk.Api.Services;


namespace ReelTalk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {

        private readonly ReelTalkDbContext _context;
        private readonly OmdbService _omdbService;


        //inyectamos nuestro DBContext a traves del constructor

        public PeliculasController(ReelTalkDbContext context, OmdbService omdbService)
        {
            _context = context;
            _omdbService = omdbService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearPelicula([FromBody] Pelicula nuevaPelicula)
        {
            // 1. Validar que el objeto no venga vacío
            if (nuevaPelicula == null)
            {
                return BadRequest("Los datos de la película no son válidos.");
            }

            // 1.1 Validar duplicados si se proporciona un ID externo (TMDB/IMDb)
            if (!string.IsNullOrWhiteSpace(nuevaPelicula.IDExternoTMDB))
            {
                bool yaExiste = await _context.Peliculas
                    .AnyAsync(p => p.IDExternoTMDB == nuevaPelicula.IDExternoTMDB);

                if (yaExiste)
                {
                    return Conflict($"Ya existe una película registrada con el ID externo '{nuevaPelicula.IDExternoTMDB}'.");
                }
            }

            // 2. Preparar el objeto de Entity Framework Core
            _context.Peliculas.Add(nuevaPelicula);

            // 3. Impactar y guardar los cambios reales en el SQL Server de forma asíncrona
            await _context.SaveChangesAsync();

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

        [HttpDelete("{id}")]
        public IActionResult BorrarPelicula(int id) 
        {
            // 1. Buscar si la película realmente existe en la base de datos
            var pelicula = _context.Peliculas.Find(id);


            // 2. Si no existe, responder con un Error 404 (Not Found)
            if (pelicula == null)
            {
                return NotFound($"No se encontró ninguna película con el ID {id}.");
            }

            // 3. Si existe, decirle a EF Core que la remueva
            _context.Peliculas.Remove(pelicula);

            // 4. Guardar los cambios permanentemente en SQL Server
            _context.SaveChanges();

            // 5. Devolver una respuesta de éxito
            return Ok(new {mensaje = $"La película '{pelicula.Titulo}' fue eliminada correctamente."});
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPeliculaPorID(int id)
        {
            // 1. Buscar la pelicula en su base de datos usando Clave Primaria (Id)
            var pelicula = _context.Peliculas.Find(id);

            // 2. Si no existe, devolver un error 404 (Not Found)
            if (pelicula == null) 
            {
                return NotFound(new { mensaje = $"No se encontró ninguna película con el ID {id}." });
            }

            // 3. Si existe, devolverla con un codigo 200 Ok
            return Ok(pelicula);
        }


        [HttpPost("importar/{imdbId}")]
        public async Task<IActionResult> ImportarPelicula(string imdbId)
        {
            // 1. Validar que el ID no esté vacío
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return BadRequest("El ID de IMDb no puede estar vacío.");
            }

            // NUEVO - 1.5: Validar si la película ya existe en SQL Server
            bool yaExiste = await _context.Peliculas
                .AnyAsync(p => p.IDExternoTMDB == imdbId);

            if (yaExiste)
            {
                return Conflict($"La película con el ID '{imdbId}' ya se encuentra importada en la base de datos.");
            }

            // 2. Consultar a OMDb
            var omdbRespuesta = await _omdbService.ObtenerPeliculaPorImdbIdAsync(imdbId);

            if (omdbRespuesta == null)
            {
                return NotFound($"No se encontró ninguna película en OMDb con el ID: {imdbId}");
            }

            // 3. Conversiones de datos para mantener coherencia
            // Convertir fecha de lanzamiento (ej: "16 Jul 2010" -> DateTime)
            DateTime.TryParse(omdbRespuesta.Released, out DateTime fechaLanzamiento);

            // Convertir duración (ej: "148 min" -> extrae solo el número 148)
            int duracionMinutos = 0;
            if (!string.IsNullOrEmpty(omdbRespuesta.Runtime))
            {
                var duracionTexto = omdbRespuesta.Runtime.Replace("min", "").Trim();
                int.TryParse(duracionTexto, out duracionMinutos);
            }

            // 4. Instanciar el objeto Pelicula con datos completos y coherentes
            var nuevaPelicula = new Pelicula(
                idexternotmdb: imdbId,
                titulo: omdbRespuesta.Title,
                sinopsis: omdbRespuesta.Plot,
                fechalanzamiento: fechaLanzamiento,
                duracionminutos: duracionMinutos,
                directores: omdbRespuesta.Director,
                actores: omdbRespuesta.Actors,
                categorias: omdbRespuesta.Genre
            );

            // 5. Guardar en SQL Server
            _context.Peliculas.Add(nuevaPelicula);
            await _context.SaveChangesAsync();

            // 6. Retornar respuesta HTTP 201 Created
            return CreatedAtAction(nameof(ObtenerPeliculaPorID), new { id = nuevaPelicula.Id }, nuevaPelicula);
        }


    }

}
