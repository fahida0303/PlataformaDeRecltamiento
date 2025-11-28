using System;

namespace ENTITY
{

    public class Candidato
    {
        public int IdCandidato { get; set; }
        public string Tipox { get; set; }
        public string NivelFormacion { get; set; }
        public string Experiencia { get; set; }
        public byte[] HojaDeVida { get; set; }


        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }
        public byte[] Foto { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string WhatsappNumber { get; set; }
    }
}