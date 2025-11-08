using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Habilidad
    {
        public int IdHabilidad { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }

        public Habilidad() { }

        public Habilidad(int idHabilidad, string nombre, string categoria)
        {
            IdHabilidad = idHabilidad;
            Nombre = nombre;
            Categoria = categoria;
        }
    }
}
