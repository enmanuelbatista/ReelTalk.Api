using System.IO;

namespace ReelTalk.Api.Modelos
{
    public class Pelicula
    {
        // Constructor VACÍO obligatorio para Entity Framework Core
        public Pelicula()
        {
        }


        public Pelicula(int idexternotmdb, string titulo, string sinopsis, DateTime fechalanzamiento, int duracionminutos, string? directores, string? actores,
            string? categorias) 
        {
            // === BLOQUE 1: EL CONSTRUCTOR ===
            // Su única función es recibir los datos que vienen del mundo exterior cuando alguien crea una película

            IDExternoTMDB = idexternotmdb;
            Titulo = titulo;
            Sinopsis = sinopsis;
            FechaLanzamiento = fechalanzamiento;
            DuracionMinutos = duracionminutos;
            Directores = directores;
            Actores = actores;
            Categorias = categorias;

        } // <- AQUÍ TERMINA EL CONSTRUCTOR. Todo lo que esté aquí dentro muere al terminar.


        // === BLOQUE 2: LAS PROPIEDADES DE LA CLASE ===
        // Estas van AFUERA del constructor, pero DENTRO de la clase.
        // Estas son las que guardan la información permanentemente en el objeto.

        public int Id { get; set; } // SQL Server necesita poder escribir aquí al generar el ID
        public int IDExternoTMDB { get; init; }
        public string Titulo { get; init; } = string.Empty;
        public string Sinopsis { get; init; } = string.Empty;
        public DateTime FechaLanzamiento { get; init; }
        public int DuracionMinutos { get; init; }
        public string? Directores { get; init; }
        public string? Actores { get; init; }
        public string? Categorias { get; init; }
    }
}
