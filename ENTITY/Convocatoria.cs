using System;

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
        public int IdReclutador { get; set; } // según tu tabla Convocatoria

        public Convocatoria()
        {
        }

        public Convocatoria(
            int idConvocatoria,
            string titulo,
            string descripcion,
            DateTime fechaPublicacion,
            DateTime fechaLimite,
            string estado,
            int idEmpresa,
            int idReclutador)
        {
            IdConvocatoria = idConvocatoria;
            Titulo = titulo;
            Descripcion = descripcion;
            FechaPublicacion = fechaPublicacion;
            FechaLimite = fechaLimite;
            Estado = estado;
            IdEmpresa = idEmpresa;
            IdReclutador = idReclutador;
        }
    }
}
