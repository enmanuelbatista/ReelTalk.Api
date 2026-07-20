using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ReelTalk.Api.DTOs; // <-- Importante: para que reconozca nuestro DTO

namespace ReelTalk.Api.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OmdbApi:ApiKey"];
            _baseUrl = configuration["OmdbApi:BaseUrl"];
        }

        // Devuelve Task de un objeto de tipo OmdbPeliculaResponse (puede ser nulo "?")
        public async Task<OmdbPeliculaResponse?> ObtenerPeliculaPorImdbIdAsync(string imdbId)
        {
            var url = $"{_baseUrl}?i={imdbId}&apikey={_apiKey}";
            var respuesta = await _httpClient.GetAsync(url);

            if (respuesta.IsSuccessStatusCode)
            {
                // 1. Leemos el JSON como texto plano de internet
                var jsonString = await respuesta.Content.ReadAsStringAsync();

                // 2. Configuramos el lector para que no sea estricto con las mayúsculas/minúsculas
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 3. Deserializamos: Convertimos el texto JSON a un objeto real de C#
                var peliculaDto = JsonSerializer.Deserialize<OmdbPeliculaResponse>(jsonString, opciones);

                // 4. OMDb a veces responde "200 OK" pero con un JSON que dice "Response: False" (si el ID no existe)
                if (peliculaDto != null && peliculaDto.Response == "True")
                {
                    return peliculaDto;
                }
            }

            return null; // Retornamos nulo si algo falló o la película no existe
        }
    }
}



