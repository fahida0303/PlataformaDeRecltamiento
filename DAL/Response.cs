using System;
using System.Collections.Generic;

namespace DAL
{
    public class Response<T>
    {
        public Response()
        {
            Estado = false;
            Mensaje = string.Empty;
            Entidad = default(T);
            Lista = null;
        }

        public Response(bool estado, string mensaje, T entidad, IList<T> lista)
        {
            Estado = estado;
            Mensaje = mensaje;
            Entidad = entidad;
            Lista = lista;
        }

        /// <summary>
        /// true = operación OK, false = error o sin resultados
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Mensaje para mostrar en logs / API / n8n
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Entidad única (para Insertar, Actualizar, ObtenerPorId, etc.)
        /// </summary>
        public T Entidad { get; set; }

        /// <summary>
        /// Lista de entidades (para ObtenerTodos, consultas, etc.)
        /// </summary>
        public IList<T> Lista { get; set; }
    }
}
