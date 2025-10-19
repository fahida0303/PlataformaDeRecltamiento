using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }

        public Usuario() { }

        public Usuario(int idUsuario, string nombre, string correo, string contrasena, string estado)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Correo = correo;
            Contrasena = contrasena;
            Estado = estado;
        }
    }
}

