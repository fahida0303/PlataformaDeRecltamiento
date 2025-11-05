using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    internal class Convocatoria
    {
        public int IdConvocatoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int IdReclutador { get; set; }
        public string Estado { get; set; }
        public DateTime FechaLimite { get; set; }

        public Convocatoria() { }

        public Convocatoria(int idConvocatoria, string titulo, string descripcion,
                            DateTime fechaPublicacion, int idReclutador, string estado,
                            DateTime fechaLimite)
        {
            IdConvocatoria = idConvocatoria;
            Titulo = titulo;
            Descripcion = descripcion;
            FechaPublicacion = fechaPublicacion;
            IdReclutador = idReclutador;
            Estado = estado;
            FechaLimite = fechaLimite;
        }
    }
}
