using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReelTalk.Api.Data;
using ReelTalk.Api.DTOs.Auth;
using ReelTalk.Api.Modelos;
using ReelTalk.Api.Services;

namespace ReelTalk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ReelTalkDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly ITokenService _tokenService;


        // Inyectamos las tres piezas que necesitamos: base de datos, hasher y generador de tokens
        public AuthController(
            ReelTalkDbContext context,
            IPasswordHasher<Usuario> passwordHasher,
            ITokenService tokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroDTO registroDto)
        {

            // 1. Validar que el objeto no venga vacio

            if (registroDto == null || string.IsNullOrWhiteSpace(registroDto.Username) || string.IsNullOrWhiteSpace(registroDto.Password))
            {
                return BadRequest("El usuario y la contraseña son obligatorios.");
            }

            // 2. Validar que el username no exista ya en la base de datos

            bool yaExiste = await _context.Usuarios.AnyAsync(u => u.Username == registroDto.Username);

            if (yaExiste)
            {
                return Conflict($"Ya existe un usuario registrado con el nombre '{registroDto.Username}'.");
            }

            // 3. Crear el objeto Usuario (todavía sin el hash real, lo llenamos abajo)

            var nuevoUsuario = new Usuario(registroDto.Username, passwordHash: string.Empty);


            // 4. Generar el hash de la contraseña y asignarlo

            nuevoUsuario.PasswordHash = _passwordHasher.HashPassword(nuevoUsuario, registroDto.Password);

            // 5. Guardar en SQL Server

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // 6. Retornar una respuesta exitosa (sin exponer el hash en la respuesta)

            return Ok(new { mensaje = "Usuario registrado con éxito", username = nuevoUsuario.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {

            // 1. Validar que el objeto no venga vacío

            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("El usuario y la contraseña son obligatorios.");
            }

            // 2. Buscar el usuario en la base de datos

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (usuario == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // 3. Verificar la contraseña comparando el hash

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, loginDto.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // 4. Generar el token JWT

            var token = _tokenService.GenerarToken(usuario);


            // 5. Retornar el token al cliente

            return Ok(new { mensaje = "Login exitoso", token = token });
        }
    }
}