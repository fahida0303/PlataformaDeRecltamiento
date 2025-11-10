using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class HistorialEstadoService
    {
        private readonly HistorialEstadoRepository _historialRepository;
        private readonly PostulacionRepository _postulacionRepository;

        public HistorialEstadoService()
        {
            _historialRepository = new HistorialEstadoRepository();
            _postulacionRepository = new PostulacionRepository();
        }

     
        public Response<HistorialEstado> RegistrarCambio(HistorialEstado historial)
        {
            try
            {
                if (historial == null)
                {
                    return new Response<HistorialEstado>(false, "El historial no puede ser nulo", null, null);
                }

                if (historial.IdPostulacion <= 0)
                {
                    return new Response<HistorialEstado>(false, "ID de postulación inválido", null, null);
                }

              
                var postulacionResponse = _postulacionRepository.ObtenerPorId(historial.IdPostulacion);
                if (!postulacionResponse.Estado)
                {
                    return new Response<HistorialEstado>(false, "Postulación no encontrada", null, null);
                }

                
                if (historial.FechaCambio == default(DateTime))
                {
                    historial.FechaCambio = DateTime.Now;
                }

                return _historialRepository.Insertar(historial);
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

        
        public Response<HistorialEstado> ObtenerHistorialPorPostulacion(int idPostulacion)
        {
            try
            {
                if (idPostulacion <= 0)
                {
                    return new Response<HistorialEstado>(false, "ID inválido", null, null);
                }

                var todosHistoriales = _historialRepository.ObtenerTodos();

                if (!todosHistoriales.Estado || todosHistoriales.Lista == null)
                {
                    return todosHistoriales;
                }

                IList<HistorialEstado> historialPostulacion = new List<HistorialEstado>();
                foreach (var hist in todosHistoriales.Lista)
                {
                    if (hist.IdPostulacion == idPostulacion)
                    {
                        historialPostulacion.Add(hist);
                    }
                }

                return new Response<HistorialEstado>(
                    true,
                    $"Se encontraron {historialPostulacion.Count} registros de historial",
                    null,
                    historialPostulacion
                );
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

       
        public Response<HistorialEstado> ObtenerUltimoCambio(int idPostulacion)
        {
            try
            {
                var historialResponse = ObtenerHistorialPorPostulacion(idPostulacion);

                if (!historialResponse.Estado || historialResponse.Lista == null || historialResponse.Lista.Count == 0)
                {
                    return new Response<HistorialEstado>(false, "No hay historial disponible", null, null);
                }

                
                HistorialEstado ultimoCambio = historialResponse.Lista[0];
                foreach (var hist in historialResponse.Lista)
                {
                    if (hist.FechaCambio > ultimoCambio.FechaCambio)
                    {
                        ultimoCambio = hist;
                    }
                }

                return new Response<HistorialEstado>(
                    true,
                    "Último cambio encontrado",
                    ultimoCambio,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

       
        public Response<HistorialEstado> ObtenerActividadPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                if (fechaInicio > fechaFin)
                {
                    return new Response<HistorialEstado>(
                        false,
                        "La fecha de inicio debe ser anterior a la fecha fin",
                        null,
                        null
                    );
                }

                var todosHistoriales = _historialRepository.ObtenerTodos();

                if (!todosHistoriales.Estado || todosHistoriales.Lista == null)
                {
                    return todosHistoriales;
                }

                IList<HistorialEstado> historialFiltrado = new List<HistorialEstado>();
                foreach (var hist in todosHistoriales.Lista)
                {
                    if (hist.FechaCambio >= fechaInicio && hist.FechaCambio <= fechaFin)
                    {
                        historialFiltrado.Add(hist);
                    }
                }

                return new Response<HistorialEstado>(
                    true,
                    $"Se encontraron {historialFiltrado.Count} cambios en el período",
                    null,
                    historialFiltrado
                );
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

       
        public Dictionary<string, int> ObtenerEstadisticasCambios()
        {
            var estadisticas = new Dictionary<string, int>
            {
                { "TotalCambios", 0 },
                { "CambiosHoy", 0 },
                { "CambiosEstaSemana", 0 },
                { "CambiosEsteMes", 0 }
            };

            try
            {
                var todosHistoriales = _historialRepository.ObtenerTodos();

                if (!todosHistoriales.Estado || todosHistoriales.Lista == null)
                {
                    return estadisticas;
                }

                DateTime hoy = DateTime.Today;
                DateTime inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
                DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

                estadisticas["TotalCambios"] = todosHistoriales.Lista.Count;

                foreach (var hist in todosHistoriales.Lista)
                {
                    if (hist.FechaCambio.Date == hoy)
                    {
                        estadisticas["CambiosHoy"]++;
                    }

                    if (hist.FechaCambio >= inicioSemana)
                    {
                        estadisticas["CambiosEstaSemana"]++;
                    }

                    if (hist.FechaCambio >= inicioMes)
                    {
                        estadisticas["CambiosEsteMes"]++;
                    }
                }
            }
            catch
            {
                // Retornar estadísticas vacías en caso de error
            }

            return estadisticas;
        }
    }
}

