using System;
using DAL;
using ENTITY;

namespace TestDAL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Prueba de conexión con la base de datos JobsyDB ===");

            try
            {
                EmpresaRepository repo = new EmpresaRepository();

                // 1️⃣ Prueba: obtener todas las empresas
                var resultado = repo.ObtenerTodos();

                if (resultado.Estado)
                {
                    Console.WriteLine(resultado.Mensaje);
                    foreach (var e in resultado.Lista)
                    {
                        Console.WriteLine($"{e.IdEmpresa}: {e.Nombre} - {e.Sector} - {e.Direccion}");
                    }
                }
                else
                {
                    Console.WriteLine("Error al obtener empresas: " + resultado.Mensaje);
                }

                // 2️⃣ Prueba opcional: insertar una nueva empresa
                Console.WriteLine("\n¿Deseas insertar una empresa de prueba? (s/n)");
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.KeyChar == 's' || key.KeyChar == 'S')
                {
                    Empresa nueva = new Empresa
                    {
                        Nombre = "Empresa Prueba DAL " + DateTime.Now.ToString("HHmmss"),
                        Sector = "Tecnología",
                        Direccion = "Calle Falsa 123",
                        CorreoContacto = "prueba@empresa.com"
                    };

                    var insert = repo.Insertar(nueva);
                    Console.WriteLine(insert.Mensaje);

                    if (insert.Estado)
                    {
                        Console.WriteLine($"✅ Nuevo ID asignado: {insert.Entidad.IdEmpresa}");

                        // 3️⃣ Volvemos a listar todas las empresas para verificar el nuevo registro
                        Console.WriteLine("\nEmpresas después de la inserción:");
                        var resultadoFinal = repo.ObtenerTodos();

                        foreach (var e in resultadoFinal.Lista)
                        {
                            Console.WriteLine($"{e.IdEmpresa}: {e.Nombre} - {e.Sector} - {e.Direccion}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Error al insertar: " + insert.Mensaje);
                    }
                }

                Console.WriteLine("\nPrueba finalizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error general: {ex.Message}");
            }

            Console.WriteLine("\nPresiona una tecla para salir...");
            Console.ReadKey();
        }
    }
}


