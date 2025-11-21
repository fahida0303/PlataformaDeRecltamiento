using System;

namespace ENTITY
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }           // PK
        public string Nombre { get; set; }           // varchar(200)
        public string Sector { get; set; }           // varchar(100)
        public string Direccion { get; set; }        // varchar(255)
        public string CorreoContacto { get; set; }   // varchar(150)

        public Empresa() { }

        public Empresa(
            int idEmpresa,
            string nombre,
            string sector,
            string direccion,
            string correoContacto
        )
        {
            IdEmpresa = idEmpresa;
            Nombre = nombre;
            Sector = sector;
            Direccion = direccion;
            CorreoContacto = correoContacto;
        }
    }
}
