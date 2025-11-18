using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Candidato : Usuario
    {
        public string Tipox { get; set; }          // Puedes usarlo luego si quieres
        public string NivelFormacion { get; set; }
        public string Experiencia { get; set; }

        // 🔹 Ahora el CV se guarda como binario:
        public byte[] HojaDeVida { get; set; }

        public Candidato() { }

        public Candidato(int idUsuario, string nombre, string correo, string contrasena,
                         string estado, string tipox, string nivelFormacion,
                         string experiencia, byte[] hojaDeVida)
            : base(idUsuario, nombre, correo, contrasena, estado)
        {
            Tipox = tipox;
            NivelFormacion = nivelFormacion;
            Experiencia = experiencia;
            HojaDeVida = hojaDeVida;
        }
    }
}
