using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
   
        public class Reunion
        {
            // 🔹 Propiedades principales (estas sí se guardan en la tabla [Reunion])
            public int IdReunion { get; set; }
            public DateTime Fecha { get; set; }
            public string EnlaceMeet { get; set; }
            public string EstadoConfirmacion { get; set; }
            public int IdPostulacion { get; set; }

            // 🔹 Propiedades derivadas (no se guardan en la BD)
            // Se completan al hacer JOIN con Postulacion, Candidato y Reclutador
            public string NombreCandidato { get; set; }
            public string CorreoCandidato { get; set; }
            public string NombreReclutador { get; set; }
            public string CorreoReclutador { get; set; }

            // 🔹 Constructores
            public Reunion() { }

            public Reunion(int idReunion, DateTime fecha, string enlaceMeet, string estadoConfirmacion, int idPostulacion)
            {
                IdReunion = idReunion;
                Fecha = fecha;
                EnlaceMeet = enlaceMeet;
                EstadoConfirmacion = estadoConfirmacion;
                IdPostulacion = idPostulacion;
            }

            public Reunion(int idReunion, DateTime fecha, string enlaceMeet, string estadoConfirmacion, int idPostulacion,
                           string nombreCandidato, string correoCandidato, string nombreReclutador, string correoReclutador)
            {
                IdReunion = idReunion;
                Fecha = fecha;
                EnlaceMeet = enlaceMeet;
                EstadoConfirmacion = estadoConfirmacion;
                IdPostulacion = idPostulacion;
                NombreCandidato = nombreCandidato;
                CorreoCandidato = correoCandidato;
                NombreReclutador = nombreReclutador;
                CorreoReclutador = correoReclutador;
            }
        }
    }



