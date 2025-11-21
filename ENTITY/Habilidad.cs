using System;

namespace ENTITY
{
    public class Habilidad
    {
        public int IdHabilidad { get; set; }   // PK
        public string Nombre { get; set; }     // varchar(100)
        public string Categoria { get; set; }  // varchar(100)

        public Habilidad() { }

        public Habilidad(int idHabilidad, string nombre, string categoria)
        {
            IdHabilidad = idHabilidad;
            Nombre = nombre;
            Categoria = categoria;
        }
    }
}
