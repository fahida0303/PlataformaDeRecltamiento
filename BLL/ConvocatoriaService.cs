using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class ConvocatoriaService // Asegúrate que sea 'public' para que el Controller lo vea
    {
        private readonly ConvocatoriaRepository _convocatoriaRepository;

        public ConvocatoriaService()
        {
            _convocatoriaRepository = new ConvocatoriaRepository();
        }

        public Response<Convocatoria> ObtenerConvocatoriasAbiertas()
        {
            try
            {
                var respuestaRepo = _convocatoriaRepository.ObtenerTodos();

                if (!respuestaRepo.Estado || respuestaRepo.Lista == null)
                {
                    return respuestaRepo; // Retornar error o lista vacía del repo
                }

                // 🔹 Lógica de Negocio Mejorada:
                // 1. Estado debe ser 'Abierta' (o 'Activa' por si acaso tienes mezcla de datos)
                // 2. Fecha Límite debe ser mayor o igual a HOY (incluyendo el día de hoy)
                var listaFiltrada = respuestaRepo.Lista
                    .Where(c => (c.Estado == "Abierta" || c.Estado == "Activa")
                             && c.FechaLimite.Date >= DateTime.Now.Date)
                    .ToList();

                if (listaFiltrada.Count == 0)
                {
                    return new Response<Convocatoria>(true, "No hay convocatorias abiertas vigentes", null, new List<Convocatoria>());
                }

                return new Response<Convocatoria>(true, "Convocatorias encontradas", null, listaFiltrada);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error en servicio: {ex.Message}", null, null);
            }
        }
    }
}