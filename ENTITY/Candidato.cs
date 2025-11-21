using System;

namespace ENTITY
{
    public class Candidato
    {
        public int IdCandidato { get; set; }         // PK
        public string Tipox { get; set; }            // varchar(50)
        public string NivelFormacion { get; set; }   // varchar(100)
        public string Experiencia { get; set; }      // varchar(255)
        public byte[] HojaDeVida { get; set; }       // varbinary(MAX)

        // FK → Usuario (Datos vinculados)
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }

        // 🟢 NUEVO: Propiedad para transportar la foto
        public byte[] Foto { get; set; }
    }
}