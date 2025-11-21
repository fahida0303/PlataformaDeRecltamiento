using BLL;
using ENTITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly CandidatoService _candidatoService;
        private readonly ReclutadorService _reclutadorService;

        public AuthController()
        {
            _usuarioService = new UsuarioService();
            _candidatoService = new CandidatoService();
            _reclutadorService = new ReclutadorService();
        }

        public class LoginRequest
        {
            public string Correo { get; set; }
            public string Contrasena { get; set; }
        }

        public class RegistroCandidatoRequest
        {
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Contrasena { get; set; }
            public string Tipox { get; set; }
            public string NivelFormacion { get; set; }
            public string Experiencia { get; set; }
            public IFormFile HojaDeVida { get; set; }
            public IFormFile Foto { get; set; } // 🟢 NUEVO
        }

        public class RegistroReclutadorRequest
        {
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Contrasena { get; set; }
            public string Cargo { get; set; }
            public int? IdEmpresa { get; set; }
        }

        // 🔐 LOGIN (Actualizado para devolver foto)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest(new { exito = false, mensaje = "Datos incompletos." });

            var respuesta = _usuarioService.ValidarCredenciales(request.Correo, request.Contrasena);

            if (!respuesta.Estado || respuesta.Entidad == null)
                return Unauthorized(new { exito = false, mensaje = respuesta.Mensaje ?? "Credenciales inválidas." });

            var usuario = respuesta.Entidad;
            var rol = string.IsNullOrWhiteSpace(usuario.TipoUsuario) ? "candidato" : usuario.TipoUsuario.ToLower();

            // 🟢 Convertir foto a Base64
            string fotoBase64 = null;
            if (usuario.Foto != null && usuario.Foto.Length > 0)
            {
                fotoBase64 = "data:image/jpeg;base64," + Convert.ToBase64String(usuario.Foto);
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Login correcto",
                usuario = new
                {
                    idUsuario = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    correo = usuario.Correo,
                    role = rol,
                    tipoUsuario = usuario.TipoUsuario,
                    telegramId = usuario.TelegramId,
                    foto = fotoBase64 // 🟢 Enviamos foto
                }
            });
        }

        // 🧑‍💼 REGISTRO CANDIDATO (Con Foto y PDF)
        [HttpPost("registro-candidato")]
        public async Task<IActionResult> RegistrarCandidato([FromForm] RegistroCandidatoRequest request)
        {
            if (request == null) return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            // Procesar PDF
            byte[] cvBytes = null;
            if (request.HojaDeVida != null && request.HojaDeVida.Length > 0)
            {
                using (var ms = new MemoryStream()) { await request.HojaDeVida.CopyToAsync(ms); cvBytes = ms.ToArray(); }
            }

            // 🟢 Procesar FOTO
            byte[] fotoBytes = null;
            if (request.Foto != null && request.Foto.Length > 0)
            {
                using (var ms = new MemoryStream()) { await request.Foto.CopyToAsync(ms); fotoBytes = ms.ToArray(); }
            }

            var candidato = new Candidato
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Contrasena = request.Contrasena,
                Tipox = request.Tipox ?? "Externo",
                NivelFormacion = request.NivelFormacion,
                Experiencia = request.Experiencia,
                HojaDeVida = cvBytes,
                Foto = fotoBytes // 🟢
            };

            var respuesta = _candidatoService.RegistrarCandidato(candidato);

            if (!respuesta.Estado) return BadRequest(new { exito = false, mensaje = respuesta.Mensaje });

            var cand = respuesta.Entidad;

            // 🟢 Convertir foto a Base64 para respuesta inmediata
            string fotoBase64 = fotoBytes != null ? "data:image/jpeg;base64," + Convert.ToBase64String(fotoBytes) : null;

            return Ok(new
            {
                exito = true,
                mensaje = respuesta.Mensaje,
                usuario = new
                {
                    idUsuario = cand.IdUsuario,
                    nombre = cand.Nombre,
                    correo = cand.Correo,
                    role = "candidato",
                    tipoUsuario = "Candidato",
                    foto = fotoBase64 // 🟢
                }
            });
        }

        // 🏢 REGISTRO RECLUTADOR
        [HttpPost("registro-reclutador")]
        public IActionResult RegistrarReclutador([FromBody] RegistroReclutadorRequest request)
        {
            // (Misma lógica que tenías antes, sin cambios necesarios aquí por ahora)
            if (request == null) return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            var reclutador = new Reclutador
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Contrasena = request.Contrasena,
                Cargo = request.Cargo,
                IdEmpresa = request.IdEmpresa ?? 0
            };

            var respuesta = _reclutadorService.RegistrarReclutador(reclutador);

            if (!respuesta.Estado) return BadRequest(new { exito = false, mensaje = respuesta.Mensaje });

            return Ok(new { exito = true, mensaje = "Registro exitoso", usuario = respuesta.Entidad });
        }
    }
}