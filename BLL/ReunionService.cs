using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class ReunionService
    {
        private readonly ReunionRepository _reunionRepository;
        private readonly CandidatoRepository _candidatoRepository;
        private readonly ReclutadorRepository _reclutadorRepository;

        public ReunionService()
        {
            _reunionRepository = new ReunionRepository();
            _candidatoRepository = new CandidatoRepository();
            _reclutadorRepository = new ReclutadorRepository();
        }

        
        public Response<Reunion> AgendarReunion(Reunion reunion)
        {
            try
            {
                
                if (reunion == null)
                {
                    return new Response<Reunion>(false, "La reunión no puede ser nula", null, null);
                }

                if (reunion.IdCandidato <= 0)
                {
                    return new Response<Reunion>(false, "Debe especificar un candidato válido", null, null);
                }

                if (reunion.IdReclutador <= 0)
                {
                    return new Response<Reunion>(false, "Debe especificar un reclutador válido", null, null);
                }

                var candidatoResponse = _candidatoRepository.ObtenerPorId(reunion.IdCandidato);
                if (!candidatoResponse.Estado)
                {
                    return new Response<Reunion>(false, "Candidato no encontrado", null, null);
                }

                var reclutadorResponse = _reclutadorRepository.ObtenerPorId(reunion.IdReclutador);
                if (!reclutadorResponse.Estado)
                {
                    return new Response<Reunion>(false, "Reclutador no encontrado", null, null);
                }


                if (reunion.Fecha <= DateTime.Now)
                {
                    return new Response<Reunion>(
                        false,
                        "La fecha de la reunión debe ser futura",
                        null,
                        null
                    );
                }

                // REGLA DE NEGOCIO: Horario laboral (8am - 6pm)
                if (reunion.Fecha.Hour < 8 || reunion.Fecha.Hour >= 18)
                {
                    return new Response<Reunion>(
                        false,
                        "Las reuniones deben ser en horario laboral (8:00 AM - 6:00 PM)",
                        null,
                        null
                    );
                }

                // Valores por defecto
                if (string.IsNullOrWhiteSpace(reunion.EstadoConfirmacion))
                {
                    reunion.EstadoConfirmacion = "Pendiente";
                }

                // Insertar en BD
                var resultado = _reunionRepository.Insertar(reunion);

                if (resultado.Estado)
                {
                    // INTEGRACIÓN FUTURA: Crear evento en Google Calendar
                    // CrearEventoCalendario(reunion);

                    // INTEGRACIÓN FUTURA: Enviar notificaciones por email
                    // EnviarNotificacionReunion(reunion);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Confirma la asistencia a una reunión.
        /// </summary>
        public Response<Reunion> ConfirmarReunion(int idReunion)
        {
            try
            {
                var reunionResponse = _reunionRepository.ObtenerPorId(idReunion);

                if (!reunionResponse.Estado)
                {
                    return new Response<Reunion>(false, "Reunión no encontrada", null, null);
                }

                var reunion = reunionResponse.Entidad;
                reunion.EstadoConfirmacion = "Confirmada";

                return _reunionRepository.Actualizar(reunion);
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Cancela una reunión.
        /// </summary>
        public Response<Reunion> CancelarReunion(int idReunion)
        {
            try
            {
                var reunionResponse = _reunionRepository.ObtenerPorId(idReunion);

                if (!reunionResponse.Estado)
                {
                    return new Response<Reunion>(false, "Reunión no encontrada", null, null);
                }

                var reunion = reunionResponse.Entidad;

                // No se puede cancelar si ya pasó
                if (reunion.Fecha < DateTime.Now)
                {
                    return new Response<Reunion>(
                        false,
                        "No se puede cancelar una reunión que ya pasó",
                        null,
                        null
                    );
                }

                reunion.EstadoConfirmacion = "Cancelada";

                return _reunionRepository.Actualizar(reunion);
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Obtiene reuniones de un candidato.
        /// </summary>
        public Response<Reunion> ObtenerReunionesPorCandidato(int idCandidato)
        {
            try
            {
                var todasReuniones = _reunionRepository.ObtenerTodos();

                if (!todasReuniones.Estado || todasReuniones.Lista == null)
                {
                    return todasReuniones;
                }

                IList<Reunion> reunionesCandidato = new List<Reunion>();
                foreach (var reunion in todasReuniones.Lista)
                {
                    if (reunion.IdCandidato == idCandidato)
                    {
                        reunionesCandidato.Add(reunion);
                    }
                }

                return new Response<Reunion>(
                    true,
                    $"Se encontraron {reunionesCandidato.Count} reuniones",
                    null,
                    reunionesCandidato
                );
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        /// <summary>
        /// Obtiene reuniones próximas (futuras).
        /// </summary>
        public Response<Reunion> ObtenerReunionesProximas()
        {
            try
            {
                var todasReuniones = _reunionRepository.ObtenerTodos();

                if (!todasReuniones.Estado || todasReuniones.Lista == null)
                {
                    return todasReuniones;
                }

                IList<Reunion> reunionesProximas = new List<Reunion>();
                foreach (var reunion in todasReuniones.Lista)
                {
                    if (reunion.Fecha > DateTime.Now)
                    {
                        reunionesProximas.Add(reunion);
                    }
                }

                return new Response<Reunion>(
                    true,
                    $"Se encontraron {reunionesProximas.Count} reuniones próximas",
                    null,
                    reunionesProximas
                );
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}
