using System.IO;

namespace ReelTalk.Api.Modelos
{
    public class Pelicula
    {
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

        public int Id { get; private set; } // La base de datos asignará este ID automáticamente después
        public int IDExternoTMDB { get; private set; }
        public string Titulo { get; private set; }
        public string Sinopsis { get; private set; }
        public DateTime FechaLanzamiento { get; private set; }
        public int DuracionMinutos { get; private set; }
        public String? Directores { get; private set; }
        public String? Actores { get; private set; }
        public String? Categorias { get; private set; }
    }
}
