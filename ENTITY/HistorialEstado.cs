using System;

namespace ENTITY
{
    public class HistorialEstado
    {
        public int IdHistorial { get; set; }      // PK
        public int IdPostulacion { get; set; }    // FK Postulacion
        public string EstadoAnterior { get; set; } // varchar(50)
        public string EstadoNuevo { get; set; }    // varchar(50)
        public DateTime FechaCambio { get; set; }  // datetime (default GETDATE())
        public string Comentario { get; set; }     // text (puede ser null)
        public int? UsuarioCambio { get; set; }    // int? (nullable)

        public HistorialEstado() { }

        public HistorialEstado(
            int idHistorial,
            int idPostulacion,
            string estadoAnterior,
            string estadoNuevo,
            DateTime fechaCambio,
            string comentario,
            int? usuarioCambio
        )
        {
            IdHistorial = idHistorial;
            IdPostulacion = idPostulacion;
            EstadoAnterior = estadoAnterior;
            EstadoNuevo = estadoNuevo;
            FechaCambio = fechaCambio;
            Comentario = comentario;
            UsuarioCambio = usuarioCambio;
        }
    }
}
