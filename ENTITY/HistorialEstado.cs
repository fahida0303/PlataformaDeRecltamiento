using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class HistorialEstado
    {
        public int IdHistorial { get; set; }
        public int IdPostulacion { get; set; }
        public string EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public string Comentario { get; set; }
        public int? UsuarioCambio { get; set; }

        public HistorialEstado() { }

        public HistorialEstado(int idHistorial, int idPostulacion, string estadoAnterior,
                               string estadoNuevo, DateTime fechaCambio, string comentario,
                               int? usuarioCambio)
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
