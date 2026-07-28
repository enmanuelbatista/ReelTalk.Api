namespace ReelTalk.Api.Modelos
{
    public class Usuario
    {
        // Constructor VACÍO obligatorio para Entity Framework Core
        public Usuario()
        {
        }

        public Usuario(string username, string passwordHash, string rol = "Usuario")
        {
            // === BLOQUE 1: EL CONSTRUCTOR ===
            // Su única función es recibir los datos que vienen del mundo exterior cuando alguien crea un usuario

            Username = username;
            PasswordHash = passwordHash;
            Rol = rol;

        } // <- AQUÍ TERMINA EL CONSTRUCTOR. Todo lo que esté aquí dentro muere al terminar.


        // === BLOQUE 2: LAS PROPIEDADES DE LA CLASE ===
        // Estas van AFUERA del constructor, pero DENTRO de la clase.
        // Estas son las que guardan la información permanentemente en el objeto.

        public int Id { get; set; } // SQL Server necesita poder escribir aquí al generar el ID
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario";
    }
}