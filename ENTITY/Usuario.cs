using System;

namespace ENTITY
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        // Nombre completo
        public string Nombre { get; set; }

        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }

        // Datos de integración con bots
        public string TelegramId { get; set; }
        public string TelegramUsername { get; set; }
        public string WhatsappNumber { get; set; }
        public DateTime? FechaUltimaInteraccionBot { get; set; }

        // Campos alineados con la BD
        public string TipoUsuario { get; set; }      // "Candidato" / "Reclutador" / "Admin"
        public string Documento { get; set; }        // cédula / ID
        public DateTime? FechaNacimiento { get; set; }
        public byte[] Foto { get; set; }             // VARBINARY(MAX)

        public Usuario() { }

        public Usuario(int idUsuario, string nombre, string correo,
                       string contrasena, string estado)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Correo = correo;
            Contrasena = contrasena;
            Estado = estado;
        }
    }
}
