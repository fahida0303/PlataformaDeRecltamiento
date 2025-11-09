using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Candidato : Usuario
    {
        public string Tipox { get; set; }  // ← AGREGAR ESTA LÍNEA
        public string NivelFormacion { get; set; }
        public string Experiencia { get; set; }
        public string HojaDeVida { get; set; }

        public Candidato() { }

        public Candidato(int idUsuario, string nombre, string correo, string contrasena,
                         string estado, string tipox, string nivelFormacion,
                         string experiencia, string hojaDeVida)
            : base(idUsuario, nombre, correo, contrasena, estado)
        {
            Tipox = tipox;
            NivelFormacion = nivelFormacion;
            Experiencia = experiencia;
            HojaDeVida = hojaDeVida;
        }
    }
}
