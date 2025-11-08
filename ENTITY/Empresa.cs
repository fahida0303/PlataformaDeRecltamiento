using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ENTITY
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Sector { get; set; }
        public string Direccion { get; set; }
        public string CorreoContacto { get; set; }
        public string Telefono { get; set; }
        public string SitioWeb { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Estado { get; set; }

        public Empresa() { }

        public Empresa(int idEmpresa, string nombre, string sector, string direccion,
                       string correoContacto, string telefono, string sitioWeb,
                       DateTime fechaRegistro, DateTime? fechaActualizacion, string estado)
        {
            IdEmpresa = idEmpresa;
            Nombre = nombre;
            Sector = sector;
            Direccion = direccion;
            CorreoContacto = correoContacto;
            Telefono = telefono;
            SitioWeb = sitioWeb;
            FechaRegistro = fechaRegistro;
            FechaActualizacion = fechaActualizacion;
            Estado = estado;
        }
    }
}
