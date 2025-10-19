using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    internal class Reunion
    {
        public int IdReunion { get; set; }
        public int IdCandidato { get; set; }
        public int IdReclutador { get; set; }
        public DateTime Fecha { get; set; }
        public string EnlaceMeet { get; set; }
        public string EstadoConfirmacion { get; set; }

        public Reunion() { }

        public Reunion(int idReunion, int idCandidato, int idReclutador,
                       DateTime fecha, string enlaceMeet, string estadoConfirmacion)
        {
            IdReunion = idReunion;
            IdCandidato = idCandidato;
            IdReclutador = idReclutador;
            Fecha = fecha;
            EnlaceMeet = enlaceMeet;
            EstadoConfirmacion = estadoConfirmacion;
        }
    }
}

