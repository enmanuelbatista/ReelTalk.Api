using System.Text.Json.Serialization;

namespace ReelTalk.Api.DTOs
{
    public class OmdbPeliculaResponse
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("Released")]
        public string Released { get; set; } = string.Empty;

        [JsonPropertyName("Runtime")]
        public string Runtime { get; set; } = string.Empty;

        [JsonPropertyName("Director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("Actors")]
        public string Actors { get; set; } = string.Empty;

        [JsonPropertyName("Genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("Response")]
        public string Response { get; set; } = string.Empty;
    }
}