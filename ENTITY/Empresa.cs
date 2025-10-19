using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    internal class Empresa
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Sector { get; set; }
        public string Direccion { get; set; }
        public string CorreoContacto { get; set; }
        public string Estado { get; set; }

        public Empresa() { }

        public Empresa(int idEmpresa, string nombre, string sector, string direccion,
                       string correoContacto, string estado)
        {
            IdEmpresa = idEmpresa;
            Nombre = nombre;
            Sector = sector;
            Direccion = direccion;
            CorreoContacto = correoContacto;
            Estado = estado;
        }
    }
}

