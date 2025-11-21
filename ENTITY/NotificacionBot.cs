using System;

namespace ENTITY
{
    public class NotificacionBot
    {
        public int IdNotificacion { get; set; }     // PK
        public int IdUsuario { get; set; }          // FK Usuario
        public string TipoNotificacion { get; set; } // varchar(50)
        public string Mensaje { get; set; }         // text
        public string EstadoEnvio { get; set; }     // varchar(20) (default 'pendiente')
        public DateTime FechaEnvio { get; set; }    // datetime (default GETDATE())

        public NotificacionBot()
        {
            EstadoEnvio = "pendiente";
            FechaEnvio = DateTime.Now;
        }

        public NotificacionBot(int idUsuario, string tipoNotificacion, string mensaje)
        {
            IdUsuario = idUsuario;
            TipoNotificacion = tipoNotificacion;
            Mensaje = mensaje;
            EstadoEnvio = "pendiente";
            FechaEnvio = DateTime.Now;
        }
    }
}
