using System.Text.Json.Serialization;

namespace ReelTalk.Api.DTOs
{
    public class OmdbPeliculaResponse
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("Response")]
        public string Response { get; set; } = string.Empty;

        
    }
}
