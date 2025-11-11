using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    internal class Postulacion
    {
        public int IdPostulacion { get; set; }
        public int IdCandidato { get; set; }
        public int IdConvocatoria { get; set; }
        public DateTime FechaPostulacion { get; set; }
        public string Estado { get; set; }

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

