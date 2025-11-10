using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class ConvocatoriaService
    {
        private readonly ConvocatoriaRepository _convocatoriaRepository;
        private readonly EmpresaRepository _empresaRepository;

        public ConvocatoriaService()
        {
            _convocatoriaRepository = new ConvocatoriaRepository();
            _empresaRepository = new EmpresaRepository();
        }

        
        public Response<Convocatoria> CrearConvocatoria(Convocatoria convocatoria)
        {
            try
            {
           
                if (convocatoria == null)
                {
                    return new Response<Convocatoria>(false, "La convocatoria no puede ser nula", null, null);
                }

                if (string.IsNullOrWhiteSpace(convocatoria.Titulo))
                {
                    return new Response<Convocatoria>(false, "El título es obligatorio", null, null);
                }

                if (string.IsNullOrWhiteSpace(convocatoria.Descripcion))
                {
                    return new Response<Convocatoria>(false, "La descripción es obligatoria", null, null);
                }

                if (convocatoria.IdEmpresa <= 0)
                {
                    return new Response<Convocatoria>(false, "Debe especificar una empresa válida", null, null);
                }

                // Verificar que la empresa existe
                var empresaResponse = _empresaRepository.ObtenerPorId(convocatoria.IdEmpresa);
                if (!empresaResponse.Estado)
                {
                    return new Response<Convocatoria>(false, "La empresa especificada no existe", null, null);
                }

                // REGLA DE NEGOCIO: Fecha límite debe ser posterior a fecha de publicación
                if (convocatoria.FechaLimite <= convocatoria.FechaPublicacion)
                {
                    return new Response<Convocatoria>(
                        false,
                        "La fecha límite debe ser posterior a la fecha de publicación",
                        null,
                        null
                    );
                }

                // REGLA DE NEGOCIO: Fecha límite debe ser futura
                if (convocatoria.FechaLimite <= DateTime.Now)
                {
                    return new Response<Convocatoria>(
                        false,
                        "La fecha límite debe ser una fecha futura",
                        null,
                        null
                    );
                }

                // Valores por defecto
                if (convocatoria.FechaPublicacion == default(DateTime))
                {
                    convocatoria.FechaPublicacion = DateTime.Now;
                }

                if (string.IsNullOrWhiteSpace(convocatoria.Estado))
                {
                    convocatoria.Estado = "Abierta";
                }

                return _convocatoriaRepository.Insertar(convocatoria);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Cierra una convocatoria (ya no acepta más postulaciones).
        /// </summary>
        public Response<Convocatoria> CerrarConvocatoria(int idConvocatoria)
        {
            try
            {
                var convocatoriaResponse = _convocatoriaRepository.ObtenerPorId(idConvocatoria);

                if (!convocatoriaResponse.Estado)
                {
                    return new Response<Convocatoria>(false, "Convocatoria no encontrada", null, null);
                }

                var convocatoria = convocatoriaResponse.Entidad;
                convocatoria.Estado = "Cerrada";

                return _convocatoriaRepository.Actualizar(convocatoria);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Obtiene convocatorias abiertas (disponibles para postularse).
        /// </summary>
        public Response<Convocatoria> ObtenerConvocatoriasAbiertas()
        {
            try
            {
                var todasConvocatorias = _convocatoriaRepository.ObtenerTodos();

                if (!todasConvocatorias.Estado || todasConvocatorias.Lista == null)
                {
                    return todasConvocatorias;
                }

                IList<Convocatoria> convocatoriasAbiertas = new List<Convocatoria>();
                foreach (var conv in todasConvocatorias.Lista)
                {
                    // Debe estar abierta y no haber pasado la fecha límite
                    if (conv.Estado == "Abierta" && conv.FechaLimite > DateTime.Now)
                    {
                        convocatoriasAbiertas.Add(conv);
                    }
                }

                return new Response<Convocatoria>(
                    true,
                    $"Se encontraron {convocatoriasAbiertas.Count} convocatorias abiertas",
                    null,
                    convocatoriasAbiertas
                );
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> ObtenerConvocatoriaPorId(int id)
        {
            try
            {
                return _convocatoriaRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> ActualizarConvocatoria(Convocatoria convocatoria)
        {
            try
            {
                if (convocatoria == null || convocatoria.IdConvocatoria <= 0)
                {
                    return new Response<Convocatoria>(false, "Datos inválidos", null, null);
                }

                return _convocatoriaRepository.Actualizar(convocatoria);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }

    // ================================================================
    // EMPRESA BLL
    // ================================================================
    /// <summary>
    /// Lógica de Negocio para Empresas.
    /// </summary>
    public class EmpresaBLL
    {
        private readonly EmpresaRepository _empresaRepository;

        public EmpresaBLL()
        {
            _empresaRepository = new EmpresaRepository();
        }

        public Response<Empresa> RegistrarEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa == null)
                {
                    return new Response<Empresa>(false, "La empresa no puede ser nula", null, null);
                }

                if (string.IsNullOrWhiteSpace(empresa.Nombre))
                {
                    return new Response<Empresa>(false, "El nombre es obligatorio", null, null);
                }

                if (string.IsNullOrWhiteSpace(empresa.CorreoContacto))
                {
                    return new Response<Empresa>(false, "El correo de contacto es obligatorio", null, null);
                }

                // Validar formato de email
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    empresa.CorreoContacto,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return new Response<Empresa>(false, "Formato de correo inválido", null, null);
                }

                return _empresaRepository.Insertar(empresa);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ActualizarEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa == null || empresa.IdEmpresa <= 0)
                {
                    return new Response<Empresa>(false, "Datos inválidos", null, null);
                }

                return _empresaRepository.Actualizar(empresa);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerEmpresaPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Empresa>(false, "ID inválido", null, null);
                }

                return _empresaRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerTodasLasEmpresas()
        {
            try
            {
                return _empresaRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> EliminarEmpresa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Empresa>(false, "ID inválido", null, null);
                }

                return _empresaRepository.Eliminar(id);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }

    // ================================================================
    // RECLUTADOR BLL
    // ================================================================
    /// <summary>
    /// Lógica de Negocio para Reclutadores.
    /// Similar a CandidatoBLL (Usuario + datos específicos).
    /// </summary>
    public class ReclutadorBLL
    {
        private readonly ReclutadorRepository _reclutadorRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly EmpresaRepository _empresaRepository;
        private readonly UsuarioService _usuarioService;

        public ReclutadorBLL()
        {
            _reclutadorRepository = new ReclutadorRepository();
            _usuarioRepository = new UsuarioRepository();
            _empresaRepository = new EmpresaRepository();
            _usuarioService = new UsuarioService();
        }

      
        public Response<Reclutador> RegistrarReclutador(Reclutador reclutador)
        {
            try
            {
                if (reclutador == null)
                {
                    return new Response<Reclutador>(false, "El reclutador no puede ser nulo", null, null);
                }

                if (reclutador.IdEmpresa <= 0)
                {
                    return new Response<Reclutador>(false, "Debe especificar una empresa válida", null, null);
                }

                var empresaResponse = _empresaRepository.ObtenerPorId(reclutador.IdEmpresa);
                if (!empresaResponse.Estado)
                {
                    return new Response<Reclutador>(false, "La empresa especificada no existe", null, null);
                }

                // Crear el Usuario primero
                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = reclutador.Nombre,
                    Correo = reclutador.Correo,
                    Contrasena = reclutador.Contrasena,
                    Estado = "Activo"
                };

                var resultadoUsuario = _usuarioService.RegistrarUsuario(nuevoUsuario);

                if (!resultadoUsuario.Estado)
                {
                    return new Response<Reclutador>(false, resultadoUsuario.Mensaje, null, null);
                }

                // Registrar datos de reclutador
                reclutador.IdUsuario = resultadoUsuario.Entidad.IdUsuario;

                var resultadoReclutador = _reclutadorRepository.Insertar(reclutador);

                if (!resultadoReclutador.Estado)
                {
                    return new Response<Reclutador>(
                        false,
                        $"Usuario creado pero error al guardar reclutador: {resultadoReclutador.Mensaje}",
                        null,
                        null
                    );
                }

                return new Response<Reclutador>(
                    true,
                    "Reclutador registrado exitosamente",
                    reclutador,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerReclutadorPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Reclutador>(false, "ID inválido", null, null);
                }

                return _reclutadorRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ActualizarReclutador(Reclutador reclutador)
        {
            try
            {
                if (reclutador == null || reclutador.IdUsuario <= 0)
                {
                    return new Response<Reclutador>(false, "Datos inválidos", null, null);
                }

                return _reclutadorRepository.Actualizar(reclutador);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerTodosLosReclutadores()
        {
            try
            {
                return _reclutadorRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

