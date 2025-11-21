using System;

namespace ENTITY
{
    public class InteraccionBot
    {
        public int IdInteraccion { get; set; }        // PK
        public int? IdUsuario { get; set; }           // FK Usuario (nullable)
        public string TelegramId { get; set; }        // varchar(50)
        public string MensajeEntrada { get; set; }    // text
        public string MensajeSalida { get; set; }     // text
        public DateTime TimestampInteraccion { get; set; } // datetime (default GETDATE())

        public InteraccionBot()
        {
            TimestampInteraccion = DateTime.Now;
        }

        public InteraccionBot(
            string telegramId,
            string mensajeEntrada,
            string mensajeSalida,
            int? idUsuario = null
        )
        {
            TelegramId = telegramId;
            MensajeEntrada = mensajeEntrada;
            MensajeSalida = mensajeSalida;
            IdUsuario = idUsuario;
            TimestampInteraccion = DateTime.Now;
        }
    }
}
