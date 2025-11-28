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
        private readonly EmpresaService _empresaService;

        public AuthController()
        {
            _usuarioService = new UsuarioService();
            _candidatoService = new CandidatoService();
            _reclutadorService = new ReclutadorService();
            _empresaService = new EmpresaService();
        }

        // ===================================================
        //                    MODELOS
        // ===================================================

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
            public IFormFile Foto { get; set; }
            public string Documento { get; set; }    
            public DateTime? FechaNacimiento { get; set; }
            public string Telefono { get; set; }
        }

        public class RegistroReclutadorRequest
        {
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Contrasena { get; set; }
            public string Cargo { get; set; }
            public string NombreEmpresa { get; set; }
        }

        // ===================================================
        //                        LOGIN
        // ===================================================

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            var respuesta = _usuarioService.ValidarCredenciales(request.Correo, request.Contrasena);

            if (!respuesta.Estado)
                return Unauthorized(new { exito = false, mensaje = respuesta.Mensaje });

            var usuario = respuesta.Entidad;

            string fotoBase64 = usuario.Foto != null
                ? "data:image/jpeg;base64," + Convert.ToBase64String(usuario.Foto)
                : null;

            return Ok(new
            {
                exito = true,
                mensaje = "Login correcto",
                usuario = new
                {
                    idUsuario = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    correo = usuario.Correo,
                    tipoUsuario = usuario.TipoUsuario,
                    role = usuario.TipoUsuario?.ToLower(),
                    telegramId = usuario.TelegramId,
                    foto = fotoBase64
                }
            });
        }

        // ===================================================
        //                REGISTRO DE CANDIDATO
        // ===================================================

        [HttpPost("registro-candidato")]
        public async Task<IActionResult> RegistrarCandidato([FromForm] RegistroCandidatoRequest request)
        {
            if (request == null)
                return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            // PDF
            byte[] cvBytes = null;
            if (request.HojaDeVida != null)
            {
                using var ms = new MemoryStream();
                await request.HojaDeVida.CopyToAsync(ms);
                cvBytes = ms.ToArray();
            }

            // FOTO
            byte[] fotoBytes = null;
            if (request.Foto != null)
            {
                using var ms = new MemoryStream();
                await request.Foto.CopyToAsync(ms);
                fotoBytes = ms.ToArray();
            }

            var candidato = new Candidato
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Contrasena = request.Contrasena,
                Tipox = request.Tipox,
                NivelFormacion = request.NivelFormacion,
                Experiencia = request.Experiencia,
                HojaDeVida = cvBytes,
                Foto = fotoBytes,
                Documento = request.Documento,
                FechaNacimiento = request.FechaNacimiento,
                WhatsappNumber = request.Telefono
            };

            var respuesta = _candidatoService.RegistrarCandidato(candidato);

            if (!respuesta.Estado)
                return BadRequest(new { exito = false, mensaje = respuesta.Mensaje });

            string fotoBase64 = fotoBytes != null
                ? "data:image/jpeg;base64," + Convert.ToBase64String(fotoBytes)
                : null;

            return Ok(new
            {
                exito = true,
                usuario = new
                {
                    idUsuario = respuesta.Entidad.IdUsuario,
                    nombre = respuesta.Entidad.Nombre,
                    correo = respuesta.Entidad.Correo,
                    tipoUsuario = "Candidato",
                    role = "candidato",
                    foto = fotoBase64
                }
            });
        }

        

        [HttpPost("registro-reclutador")]
        public IActionResult RegistrarReclutador([FromBody] RegistroReclutadorRequest request)
        {
            if (request == null)
                return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            // 1️⃣ Crear empresa
            var empresa = new Empresa
            {
                Nombre = request.NombreEmpresa,
                Sector = "",
                Direccion = "",
                CorreoContacto = request.Correo
            };

            var respEmpresa = _empresaService.RegistrarEmpresa(empresa);
            if (!respEmpresa.Estado)
                return BadRequest(new { exito = false, mensaje = respEmpresa.Mensaje });

            int idEmpresa = respEmpresa.Entidad.IdEmpresa;

            // 2️⃣ Crear usuario
            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Correo = request.Correo,
                Contrasena = request.Contrasena,
                Estado = "Activo",
                TipoUsuario = "Reclutador"
            };

            var respUsuario = _usuarioService.RegistrarUsuario(usuario);
            if (!respUsuario.Estado)
                return BadRequest(new { exito = false, mensaje = respUsuario.Mensaje });

            // 3️⃣ Crear reclutador vinculado al usuario y empresa
            var reclutador = new Reclutador
            {
                IdUsuario = respUsuario.Entidad.IdUsuario,
                Cargo = request.Cargo,
                IdEmpresa = idEmpresa
            };

            var respReclutador = _reclutadorService.RegistrarReclutador(reclutador);
            if (!respReclutador.Estado)
                return BadRequest(new { exito = false, mensaje = respReclutador.Mensaje });

            return Ok(new
            {
                exito = true,
                mensaje = "Reclutador registrado con éxito",
                usuario = new
                {
                    idUsuario = respUsuario.Entidad.IdUsuario,
                    nombre = request.Nombre,
                    correo = request.Correo,
                    tipoUsuario = "Reclutador",
                    cargo = request.Cargo
                },
                idEmpresa
            });
        }
    }
}
