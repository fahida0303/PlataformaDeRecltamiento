using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class NotificacionBot
    {
       
       
            public int IdNotificacion { get; set; }
            public int IdUsuario { get; set; }
            public string TipoNotificacion { get; set; }
            public string Mensaje { get; set; }
            public string EstadoEnvio { get; set; }
            public DateTime FechaEnvio { get; set; }

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

