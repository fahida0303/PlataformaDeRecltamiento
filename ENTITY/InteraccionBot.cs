using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class InteraccionBot
    {
        public int IdInteraccion { get; set; }
        public int? IdUsuario { get; set; }
        public string TelegramId { get; set; }
        public string MensajeEntrada { get; set; }
        public string MensajeSalida { get; set; }
        public DateTime TimestampInteraccion { get; set; }

        public InteraccionBot()
        {
            TimestampInteraccion = DateTime.Now;
        }

        public InteraccionBot(string telegramId, string mensajeEntrada,
                             string mensajeSalida, int? idUsuario = null)
        {
            TelegramId = telegramId;
            MensajeEntrada = mensajeEntrada;
            MensajeSalida = mensajeSalida;
            IdUsuario = idUsuario;
            TimestampInteraccion = DateTime.Now;
        }
    }
}
