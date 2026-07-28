using ReelTalk.Api.Modelos;


namespace ReelTalk.Api.Services
{
    public interface ITokenService
    {

        string GenerarToken(Usuario usuario);
    }
}
