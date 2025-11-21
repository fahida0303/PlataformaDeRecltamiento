
using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.IO;

namespace BLL
{
    public class CandidatoService
    {
        private readonly CandidatoRepository _candidatoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly UsuarioService _usuarioService;

        public CandidatoService()
        {
            _candidatoRepository = new CandidatoRepository();
            _usuarioRepository = new UsuarioRepository();
            _usuarioService = new UsuarioService();
        }

        public Response<Candidato> RegistrarCandidato(Candidato candidato)
        {
            try
            {
                if (candidato == null) return new Response<Candidato>(false, "El candidato no puede ser nulo", null, null);
                if (string.IsNullOrWhiteSpace(candidato.Nombre)) return new Response<Candidato>(false, "El nombre es obligatorio", null, null);
                if (string.IsNullOrWhiteSpace(candidato.Correo)) return new Response<Candidato>(false, "El correo es obligatorio", null, null);
                if (string.IsNullOrWhiteSpace(candidato.Contrasena)) return new Response<Candidato>(false, "La contraseña es obligatoria", null, null);

                // 1) Crear usuario base con la FOTO
                var nuevoUsuario = new Usuario
                {
                    Nombre = candidato.Nombre,
                    Correo = candidato.Correo,
                    Contrasena = candidato.Contrasena,
                    Estado = "Activo",
                    TipoUsuario = "Candidato",
                    Foto = candidato.Foto // 🟢 ASIGNAMOS LA FOTO AQUÍ
                };

                var resultadoUsuario = _usuarioService.RegistrarUsuario(nuevoUsuario);

                if (!resultadoUsuario.Estado)
                {
                    return new Response<Candidato>(false, resultadoUsuario.Mensaje, null, null);
                }

                // 2) Crear perfil candidato
                candidato.IdUsuario = resultadoUsuario.Entidad.IdUsuario;
                if (string.IsNullOrWhiteSpace(candidato.Tipox)) candidato.Tipox = "Empleado";

                var resultadoCandidato = _candidatoRepository.Insertar(candidato);

                if (!resultadoCandidato.Estado)
                {
                    return new Response<Candidato>(false, $"Usuario creado pero error en candidato: {resultadoCandidato.Mensaje}", null, null);
                }

                return new Response<Candidato>(true, "Candidato registrado exitosamente", candidato, null);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al registrar candidato: {ex.Message}", null, null);
            }
        }
        

        /// <summary>
        /// Actualiza datos generales del perfil del candidato.
        /// </summary>
        public Response<Candidato> ActualizarPerfil(Candidato candidato)
        {
            try
            {
                if (candidato == null || candidato.IdUsuario <= 0)
                {
                    return new Response<Candidato>(false, "Datos inválidos", null, null);
                }

                var candidatoExistente = _candidatoRepository.ObtenerPorId(candidato.IdCandidato);
                if (!candidatoExistente.Estado)
                {
                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }

                // Validar nivel de formación (opcional)
                if (!string.IsNullOrWhiteSpace(candidato.NivelFormacion))
                {
                    var nivelesValidos = new List<string>
                    {
                        "Secundaria",
                        "Técnico",
                        "Tecnólogo",
                        "Universitario",
                        "Especialización",
                        "Maestría",
                        "Doctorado"
                    };

                    if (!nivelesValidos.Contains(candidato.NivelFormacion))
                    {
                        return new Response<Candidato>(
                            false,
                            "Nivel de formación no válido",
                            null,
                            null
                        );
                    }
                }

                return _candidatoRepository.Actualizar(candidato);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        /// <summary>
        /// Actualiza la hoja de vida (archivo físico convertido a byte[]) de un candidato.
        /// </summary>
        public Response<Candidato> ActualizarHojaDeVida(int idCandidato, string rutaHojaDeVida)
        {
            try
            {
                if (idCandidato <= 0)
                {
                    return new Response<Candidato>(false, "ID inválido", null, null);
                }

                if (string.IsNullOrWhiteSpace(rutaHojaDeVida))
                {
                    return new Response<Candidato>(
                        false,
                        "La ruta de la hoja de vida es requerida",
                        null,
                        null
                    );
                }

                var candidatoResponse = _candidatoRepository.ObtenerPorId(idCandidato);
                if (!candidatoResponse.Estado)
                {
                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }

                var candidato = candidatoResponse.Entidad;

                // Leer archivo físico a byte[]
                byte[] contenidoHV;
                try
                {
                    contenidoHV = File.ReadAllBytes(rutaHojaDeVida);
                }
                catch (Exception ex)
                {
                    return new Response<Candidato>(
                        false,
                        "No se pudo leer el archivo de hoja de vida: " + ex.Message,
                        null,
                        null
                    );
                }

                candidato.HojaDeVida = contenidoHV;

                return _candidatoRepository.Actualizar(candidato);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        public Response<Candidato> ObtenerCandidatoPorId(int idCandidato)
        {
            try
            {
                if (idCandidato <= 0)
                {
                    return new Response<Candidato>(false, "ID inválido", null, null);
                }

                return _candidatoRepository.ObtenerPorId(idCandidato);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        public Response<Candidato> ObtenerTodosLosCandidatos()
        {
            try
            {
                return _candidatoRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        public Response<Candidato> ObtenerCandidatosActivos()
        {
            try
            {
                var todosCandidatos = _candidatoRepository.ObtenerTodos();

                if (!todosCandidatos.Estado || todosCandidatos.Lista == null)
                {
                    return todosCandidatos;
                }

                var candidatosActivos = new List<Candidato>();
                foreach (var candidato in todosCandidatos.Lista)
                {
                    if (candidato.Estado == "Activo")
                    {
                        candidatosActivos.Add(candidato);
                    }
                }

                return new Response<Candidato>(
                    true,
                    $"Se encontraron {candidatosActivos.Count} candidatos activos",
                    null,
                    candidatosActivos
                );
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        /// <summary>
        /// Verifica si el perfil del candidato está completo para poder postularse.
        /// </summary>
        public Response<Candidato> ValidarPerfilCompleto(int idCandidato)
        {
            try
            {
                var candidatoResponse = _candidatoRepository.ObtenerPorId(idCandidato);

                if (!candidatoResponse.Estado)
                {
                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }

                var candidato = candidatoResponse.Entidad;

                if (string.IsNullOrWhiteSpace(candidato.NivelFormacion))
                {
                    return new Response<Candidato>(
                        false,
                        "Debe completar su nivel de formación",
                        null,
                        null
                    );
                }

                if (string.IsNullOrWhiteSpace(candidato.Experiencia))
                {
                    return new Response<Candidato>(
                        false,
                        "Debe completar su experiencia laboral",
                        null,
                        null
                    );
                }

                if (candidato.HojaDeVida == null || candidato.HojaDeVida.Length == 0)
                {
                    return new Response<Candidato>(
                        false,
                        "Debe cargar su hoja de vida",
                        null,
                        null
                    );
                }

                return new Response<Candidato>(
                    true,
                    "Perfil completo",
                    candidato,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }

        /// <summary>
        /// “Desactivar” candidato (en tu repo lo manejas como Eliminar).
        /// </summary>
        public Response<Candidato> DesactivarCandidato(int idCandidato)
        {
            try
            {
                if (idCandidato <= 0)
                {
                    return new Response<Candidato>(false, "ID inválido", null, null);
                }

                var candidatoResponse = _candidatoRepository.ObtenerPorId(idCandidato);
                if (!candidatoResponse.Estado)
                {
                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }

                return _candidatoRepository.Eliminar(idCandidato);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(
                    false,
                    $"Error: {ex.Message}",
                    null,
                    null
                );
            }
        }
    }
}
