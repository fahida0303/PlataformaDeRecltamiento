using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class HabilidadesService
    {
        private readonly HabilidadRepository _habilidadRepository;
        private readonly CandidatoHabilidadRepository _candidatoHabilidadRepository;

        public HabilidadesService()
        {
            _habilidadRepository = new HabilidadRepository();
            _candidatoHabilidadRepository = new CandidatoHabilidadRepository();
        }

        public Response<Habilidad> RegistrarHabilidad(Habilidad habilidad)
        {
            try
            {
                if (habilidad == null || string.IsNullOrWhiteSpace(habilidad.Nombre))
                {
                    return new Response<Habilidad>(false, "El nombre es obligatorio", null, null);
                }

                return _habilidadRepository.Insertar(habilidad);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerTodasLasHabilidades()
        {
            try
            {
                return _habilidadRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> AsignarHabilidadACandidato(CandidatoHabilidad candidatoHabilidad)
        {
            try
            {
                if (candidatoHabilidad == null)
                {
                    return new Response<CandidatoHabilidad>(false, "Datos inválidos", null, null);
                }

                var nivelesValidos = new List<string> { "Básico", "Intermedio", "Avanzado", "Experto" };

                if (!string.IsNullOrWhiteSpace(candidatoHabilidad.NivelDominio) &&
                    !nivelesValidos.Contains(candidatoHabilidad.NivelDominio))
                {
                    return new Response<CandidatoHabilidad>(
                        false,
                        "Nivel de dominio no válido. Use: Básico, Intermedio, Avanzado o Experto",
                        null,
                        null
                    );
                }

                return _candidatoHabilidadRepository.Insertar(candidatoHabilidad);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

