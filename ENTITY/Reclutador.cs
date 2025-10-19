using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    // Hereda de Usuario
    public class Reclutador : Usuario
    {
        public string Cargo { get; set; }
        public int IdEmpresa { get; set; }

        public Reclutador() { }

        public Reclutador(int idUsuario, string nombre, string correo, string contrasena,
                          string estado, int idReclutador, string cargo, int idEmpresa)
            : base(idUsuario, nombre, correo, contrasena, estado)
        {

            Cargo = cargo;
            IdEmpresa = idEmpresa;
        }
    }
}

