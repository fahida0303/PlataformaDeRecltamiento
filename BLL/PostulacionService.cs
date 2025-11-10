using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class PostulacionService
    {
        private readonly PostulacionRepository _postulacionRepository;
        private readonly ConvocatoriaRepository _convocatoriaRepository;
        private readonly CandidatoRepository _candidatoRepository;
        private readonly HistorialEstadoRepository _historialRepository;

        public PostulacionService()
        {
            _postulacionRepository = new PostulacionRepository();
            _convocatoriaRepository = new ConvocatoriaRepository();
            _candidatoRepository = new CandidatoRepository();
            _historialRepository = new HistorialEstadoRepository();
        }

        public Response<Postulacion> RegistrarPostulacion(Postulacion postulacion)
        {
            try
            {

                if (postulacion == null)
                {
                    return new Response<Postulacion>(false, "La postulación no puede ser nula", null, null);
                }

                if (postulacion.IdCandidato <= 0)
                {
                    return new Response<Postulacion>(false, "ID de candidato inválido", null, null);
                }

                if (postulacion.IdConvocatoria <= 0)
                {
                    return new Response<Postulacion>(false, "ID de convocatoria inválido", null, null);
                }

                var candidatoResponse = _candidatoRepository.ObtenerPorId(postulacion.IdCandidato);
                if (!candidatoResponse.Estado)
                {
                    return new Response<Postulacion>(false, "Candidato no encontrado", null, null);
                }

                if (candidatoResponse.Entidad.Estado != "Activo")
                {
                    return new Response<Postulacion>(
                        false,
                        "No puede postularse con un perfil inactivo",
                        null,
                        null
                    );
                }

                var candidato = candidatoResponse.Entidad;
                if (string.IsNullOrWhiteSpace(candidato.HojaDeVida))
                {
                    return new Response<Postulacion>(
                        false,
                        "Debe cargar su hoja de vida antes de postularse",
                        null,
                        null
                    );
                }


                var convocatoriaResponse = _convocatoriaRepository.ObtenerPorId(postulacion.IdConvocatoria);
                if (!convocatoriaResponse.Estado)
                {
                    return new Response<Postulacion>(false, "Convocatoria no encontrada", null, null);
                }

                var convocatoria = convocatoriaResponse.Entidad;


                if (convocatoria.Estado != "Abierta")
                {
                    return new Response<Postulacion>(
                        false,
                        "Esta convocatoria ya no está abierta",
                        null,
                        null
                    );
                }


                if (DateTime.Now > convocatoria.FechaLimite)
                {
                    return new Response<Postulacion>(
                        false,
                        "La fecha límite de esta convocatoria ya pasó",
                        null,
                        null
                    );
                }


                if (YaEstaPostulado(postulacion.IdCandidato, postulacion.IdConvocatoria))
                {
                    return new Response<Postulacion>(
                        false,
                        "Ya te has postulado a esta convocatoria anteriormente",
                        null,
                        null
                    );
                }


                postulacion.FechaPostulacion = DateTime.Now;
                postulacion.Estado = "En revisión"; // Estado inicial


                var resultado = _postulacionRepository.Insertar(postulacion);

                if (resultado.Estado)
                {

                    RegistrarHistorial(
                        resultado.Entidad.IdPostulacion,
                        null,
                        "En revisión",
                        "Postulación registrada"
                    );
                }

                return resultado;
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }


        /// ESTADOS VÁLIDOS: En revisión, Preseleccionado, Entrevista, Aceptado, Rechazado.

        public Response<Postulacion> CambiarEstadoPostulacion(
            int idPostulacion,
            string nuevoEstado,
            string comentario,
            int idUsuarioCambio)
        {
            try
            {

                if (idPostulacion <= 0)
                {
                    return new Response<Postulacion>(false, "ID inválido", null, null);
                }


                var estadosValidos = new List<string>
                {
                    "En revisión",
                    "Preseleccionado",
                    "Entrevista",
                    "Aceptado",
                    "Rechazado"
                };

                if (!estadosValidos.Contains(nuevoEstado))
                {
                    return new Response<Postulacion>(
                        false,
                        "Estado no válido. Use: En revisión, Preseleccionado, Entrevista, Aceptado o Rechazado",
                        null,
                        null
                    );
                }

                var postulacionResponse = _postulacionRepository.ObtenerPorId(idPostulacion);
                if (!postulacionResponse.Estado)
                {
                    return new Response<Postulacion>(false, "Postulación no encontrada", null, null);
                }

                var postulacion = postulacionResponse.Entidad;
                string estadoAnterior = postulacion.Estado;


                if (estadoAnterior == "Aceptado" || estadoAnterior == "Rechazado")
                {
                    return new Response<Postulacion>(
                        false,
                        "No se puede modificar una postulación que ya fue finalizada",
                        null,
                        null
                    );
                }


                postulacion.Estado = nuevoEstado;
                var resultado = _postulacionRepository.Actualizar(postulacion);

                if (resultado.Estado)
                {

                    RegistrarHistorial(
                        idPostulacion,
                        estadoAnterior,
                        nuevoEstado,
                        comentario,
                        idUsuarioCambio
                    );

                    // NOTA: Aquí podrías enviar notificación al candidato
                    // EnviarNotificacionCambioEstado(postulacion, nuevoEstado);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }


        public Response<Postulacion> ObtenerPostulacionesPorCandidato(int idCandidato)
        {
            try
            {
                if (idCandidato <= 0)
                {
                    return new Response<Postulacion>(false, "ID inválido", null, null);
                }

                var todasPostulaciones = _postulacionRepository.ObtenerTodos();

                if (!todasPostulaciones.Estado || todasPostulaciones.Lista == null)
                {
                    return todasPostulaciones;
                }


                IList<Postulacion> postulacionesCandidato = new List<Postulacion>();
                foreach (var post in todasPostulaciones.Lista)
                {
                    if (post.IdCandidato == idCandidato)
                    {
                        postulacionesCandidato.Add(post);
                    }
                }

                return new Response<Postulacion>(
                    true,
                    $"Se encontraron {postulacionesCandidato.Count} postulaciones",
                    null,
                    postulacionesCandidato
                );
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerPostulacionesPorConvocatoria(int idConvocatoria)
        {
            try
            {
                if (idConvocatoria <= 0)
                {
                    return new Response<Postulacion>(false, "ID inválido", null, null);
                }

                var todasPostulaciones = _postulacionRepository.ObtenerTodos();

                if (!todasPostulaciones.Estado || todasPostulaciones.Lista == null)
                {
                    return todasPostulaciones;
                }

                IList<Postulacion> postulacionesConvocatoria = new List<Postulacion>();
                foreach (var post in todasPostulaciones.Lista)
                {
                    if (post.IdConvocatoria == idConvocatoria)
                    {
                        postulacionesConvocatoria.Add(post);
                    }
                }

                return new Response<Postulacion>(
                    true,
                    $"Se encontraron {postulacionesConvocatoria.Count} postulaciones",
                    null,
                    postulacionesConvocatoria
                );
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Dictionary<string, int> ObtenerEstadisticasConvocatoria(int idConvocatoria)
        {
            var estadisticas = new Dictionary<string, int>
            {
                { "Total", 0 },
                { "En revisión", 0 },
                { "Preseleccionado", 0 },
                { "Entrevista", 0 },
                { "Aceptado", 0 },
                { "Rechazado", 0 }
            };

            try
            {
                var postulaciones = ObtenerPostulacionesPorConvocatoria(idConvocatoria);

                if (!postulaciones.Estado || postulaciones.Lista == null)
                {
                    return estadisticas;
                }

                estadisticas["Total"] = postulaciones.Lista.Count;

                foreach (var post in postulaciones.Lista)
                {
                    if (estadisticas.ContainsKey(post.Estado))
                    {
                        estadisticas[post.Estado]++;
                    }
                }
            }
            catch
            {
                // Retornar estadísticas vacías en caso de error
            }

            return estadisticas;
        }



        private bool YaEstaPostulado(int idCandidato, int idConvocatoria)
        {
            try
            {
                var todasPostulaciones = _postulacionRepository.ObtenerTodos();

                if (!todasPostulaciones.Estado || todasPostulaciones.Lista == null)
                {
                    return false;
                }

                foreach (var post in todasPostulaciones.Lista)
                {
                    if (post.IdCandidato == idCandidato && post.IdConvocatoria == idConvocatoria)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        private void RegistrarHistorial(
            int idPostulacion,
            string estadoAnterior,
            string estadoNuevo,
            string comentario,
            int? idUsuarioCambio = null)
        {
            try
            {
                var historial = new HistorialEstado
                {
                    IdPostulacion = idPostulacion,
                    EstadoAnterior = estadoAnterior ?? "Ninguno",
                    EstadoNuevo = estadoNuevo,
                    FechaCambio = DateTime.Now,
                    Comentario = comentario,
                    UsuarioCambio = idUsuarioCambio
                };

                _historialRepository.Insertar(historial);
            }
            catch
            {
                // Si falla el historial, no interrumpimos el flujo principal
            }
        }
    }
}

