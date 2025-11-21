using System;

namespace ENTITY
{
    public class Postulacion
    {
        // 🔹 Campos que SÍ existen en la tabla Postulacion
        public int IdPostulacion { get; set; }
        public int IdCandidato { get; set; }
        public int IdConvocatoria { get; set; }
        public DateTime FechaPostulacion { get; set; }
        public string Estado { get; set; }
        public decimal? Score { get; set; }        // score (decimal, puede ser NULL)
        public string Justificacion { get; set; }  // justificacion (nvarchar, puede ser NULL)


        public string NombreCandidato { get; set; }
        public string CorreoCandidato { get; set; }   // 👈 ESTA es la que faltaba
        public string TelefonoCandidato { get; set; }
        public string TituloConvocatoria { get; set; }
        public string NombreEmpresa { get; set; }
        public byte[] HojaDeVida { get; set; }        // varbinary de Candidato.hojaDeVida

        public Postulacion()
        {
        }

        public Postulacion(
            int idPostulacion,
            int idCandidato,
            int idConvocatoria,
            DateTime fechaPostulacion,
            string estado)
        {
            IdPostulacion = idPostulacion;
            IdCandidato = idCandidato;
            IdConvocatoria = idConvocatoria;
            FechaPostulacion = fechaPostulacion;
            Estado = estado;
        }
    }
}
