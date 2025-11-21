using System;

namespace ENTITY
{
    public class Reunion
    {
        public int IdReunion { get; set; }
        public DateTime Fecha { get; set; }
        public string EnlaceMeet { get; set; }
        public string EstadoConfirmacion { get; set; }

        public int IdCandidato { get; set; }
        public int IdReclutador { get; set; }

        // Campos adicionales de la tabla
        public string IdEventoCalendar { get; set; }   // nvarchar(510) NULL
        public DateTime? FechaModificacion { get; set; } // datetime NULL (default GETDATE())

        // Propiedades derivadas (para JOINs, no columnas directas)
        public string NombreCandidato { get; set; }
        public string CorreoCandidato { get; set; }
        public string NombreReclutador { get; set; }
        public string CorreoReclutador { get; set; }

        public Reunion() { }

        public Reunion(
            int idReunion,
            DateTime fecha,
            string enlaceMeet,
            string estadoConfirmacion,
            int idCandidato,
            int idReclutador,
            string idEventoCalendar = null,
            DateTime? fechaModificacion = null
        )
        {
            IdReunion = idReunion;
            Fecha = fecha;
            EnlaceMeet = enlaceMeet;
            EstadoConfirmacion = estadoConfirmacion;
            IdCandidato = idCandidato;
            IdReclutador = idReclutador;
            IdEventoCalendar = idEventoCalendar;
            FechaModificacion = fechaModificacion;
        }
    }
}
