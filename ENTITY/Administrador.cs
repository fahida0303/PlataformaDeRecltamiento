using System;

namespace ENTITY
{
    // Representa la fila de Administrador y extiende la info de Usuario
    public class Administrador : Usuario
    {
        public int IdAdmin { get; set; }
        public string Permisos { get; set; }

        public Administrador() { }

        public Administrador(
            int idUsuario,
            string nombre,
            string correo,
            string contrasena,
            string estado,
            int idAdmin,
            string permisos
        ) : base(idUsuario, nombre, correo, contrasena, estado)
        {
            IdAdmin = idAdmin;
            Permisos = permisos;
        }
    }
}
