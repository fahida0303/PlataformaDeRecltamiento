using System;

namespace ENTITY
{
    // Hereda de Usuario y añade info propia de la tabla Reclutador
    public class Reclutador : Usuario
    {
        public int IdReclutador { get; set; }  // PK de la tabla Reclutador
        public string Cargo { get; set; }      // varchar(100)
        public int IdEmpresa { get; set; }     // FK a Empresa (empresaAsociada)

        public Reclutador() { }

        public Reclutador(
            int idUsuario,
            string nombre,
            string correo,
            string contrasena,
            string estado,
            int idReclutador,
            string cargo,
            int idEmpresa
        ) : base(idUsuario, nombre, correo, contrasena, estado)
        {
            IdReclutador = idReclutador;
            Cargo = cargo;
            IdEmpresa = idEmpresa;
        }
    }
}
