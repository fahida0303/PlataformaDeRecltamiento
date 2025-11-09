using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Convocatoria
    {
        public int IdConvocatoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdReclutador { get; set; }  

        public Convocatoria() { }

        public Convocatoria(int idConvocatoria, string titulo, string descripcion,
                            DateTime fechaPublicacion, DateTime fechaLimite,
                            string estado, int idEmpresa)
        {
            IdConvocatoria = idConvocatoria;
            Titulo = titulo;
            Descripcion = descripcion;
            FechaPublicacion = fechaPublicacion;
            FechaLimite = fechaLimite;
            Estado = estado;
            IdEmpresa = idEmpresa;
        }
    }
}
