using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Reunion
    {
        public int IdReunion { get; set; }
        public DateTime Fecha { get; set; }
        public string EnlaceMeet { get; set; }
        public string EstadoConfirmacion { get; set; }

        // ← AGREGAR ESTAS DOS LÍNEAS:
        public int IdCandidato { get; set; }
        public int IdReclutador { get; set; }

        // Propiedades derivadas (para JOIN)
        public string NombreCandidato { get; set; }
        public string CorreoCandidato { get; set; }
        public string NombreReclutador { get; set; }
        public string CorreoReclutador { get; set; }

        public Reunion() { }

        public Reunion(int idReunion, DateTime fecha, string enlaceMeet,
                       string estadoConfirmacion, int idCandidato, int idReclutador)
        {
            IdReunion = idReunion;
            Fecha = fecha;
            EnlaceMeet = enlaceMeet;
            EstadoConfirmacion = estadoConfirmacion;
            IdCandidato = idCandidato;
            IdReclutador = idReclutador;
        }
    }
}



