using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    // Hereda de Usuario
    public class Candidato : Usuario
    {
        public string NivelFormacion { get; set; }
        public string Experiencia { get; set; }
        public string HojaDeVida { get; set; }

        public Candidato() { }

        public Candidato(int idUsuario, string nombre, string correo, string contrasena,
                         string estado, int idCandidato, string nivelFormacion,
                         string experiencia, string hojaDeVida)
            : base(idUsuario, nombre, correo, contrasena, estado)
        {
            NivelFormacion = nivelFormacion;
            Experiencia = experiencia;
            HojaDeVida = hojaDeVida;
        }
    }
}
