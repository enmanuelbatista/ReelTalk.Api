using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReelTalk.Api.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // El constructor recibe el HttpClient e inyecta la configuración (Secret Manager)
        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OmdbApi:ApiKey"];
            _baseUrl = configuration["OmdbApi:BaseUrl"];
        }

        // Método asíncrono para buscar una película en OMDb usando su ID de IMDb (ej: tt0111161)
        public async Task<string> ObtenerPeliculaPorImdbIdAsync(string imdbId)
        {
            // Construimos la URL uniendo la base, el ID de la película (i) y tu clave secreta (apikey)
            var url = $"{_baseUrl}?i={imdbId}&apikey={_apiKey}";

            // Realizamos la petición HTTP GET de forma asíncrona
            var respuesta = await _httpClient.GetAsync(url);

            if (respuesta.IsSuccessStatusCode)
            {
                // Si el servidor de OMDb responde bien (200 OK), leemos el JSON crudo como texto
                return await respuesta.Content.ReadAsStringAsync();
            }

            return null;

        }
}

}
