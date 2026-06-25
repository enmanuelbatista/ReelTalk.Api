using Microsoft.EntityFrameworkCore;
using ReelTalk.Api.Modelos;

namespace ReelTalk.Api.Data
{
    // Heredamos de DbContext para que EF Core sepa que esta clase controla la Base de Datos
    public class ReelTalkDbContext : DbContext
    {
      public ReelTalkDbContext(DbContextOptions<ReelTalkDbContext> options) : base(options) 
        { 


        }

        // Esta propiedad le dice a EF Core: "Crea una tabla llamada 'Peliculas' basada en la clase Pelicula"
        public DbSet<Pelicula> Peliculas { get; set; }
    }
}
