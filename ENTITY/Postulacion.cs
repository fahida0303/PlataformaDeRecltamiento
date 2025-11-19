using System;

namespace ENTITY
{
    public class Postulacion
    {
        public int IdPostulacion { get; set; }
        public int IdCandidato { get; set; }
        public int IdConvocatoria { get; set; }
        public DateTime FechaPostulacion { get; set; }
        public string Estado { get; set; }
        public decimal? Score { get; set; } // ⚠️ Cambié de int? a decimal? para precisión
        public string Justificacion { get; set; }

        // 🔹 Propiedades adicionales para la API (no se guardan en BD)
        public string NombreCandidato { get; set; }
        public string Correo { get; set; }
        public byte[] HojaDeVida { get; set; }

        public Postulacion() { }

        public Postulacion(int idPostulacion, int idCandidato, int idConvocatoria,
                           DateTime fechaPostulacion, string estado)
        {
            IdPostulacion = idPostulacion;
            IdCandidato = idCandidato;
            IdConvocatoria = idConvocatoria;
            FechaPostulacion = fechaPostulacion;
            Estado = estado;
        }
    }
}