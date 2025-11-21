using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AdministradorService
    {
        private readonly AdministradorRepository _administradorRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly UsuarioService _usuarioService;

        public AdministradorService()
        {
            _administradorRepository = new AdministradorRepository();
            _usuarioRepository = new UsuarioRepository();
            _usuarioService = new UsuarioService();
        }

       
        public Response<Administrador> RegistrarAdministrador(Administrador administrador, int idAdminCreador)
        {
            try
            {
                if (administrador == null)
                {
                    return new Response<Administrador>(false, "El administrador no puede ser nulo", null, null);
                }

                
                var adminCreador = _administradorRepository.ObtenerPorId(idAdminCreador);
                if (!adminCreador.Estado)
                {
                    return new Response<Administrador>(
                        false,
                        "Solo un administrador puede crear otros administradores",
                        null,
                        null
                    );
                }

                if (string.IsNullOrWhiteSpace(administrador.Permisos))
                {
                    administrador.Permisos = "Lectura, Escritura"; // Permisos por defecto
                }

                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = administrador.Nombre,
                    Correo = administrador.Correo,
                    Contrasena = administrador.Contrasena,
                    Estado = "Activo"
                };

                var resultadoUsuario = _usuarioService.RegistrarUsuario(nuevoUsuario);

                if (!resultadoUsuario.Estado)
                {
                    return new Response<Administrador>(false, resultadoUsuario.Mensaje, null, null);
                }


                administrador.IdUsuario = resultadoUsuario.Entidad.IdUsuario;

                var resultadoAdmin = _administradorRepository.Insertar(administrador);

                if (!resultadoAdmin.Estado)
                {
                    return new Response<Administrador>(
                        false,
                        $"Usuario creado pero error al guardar admin: {resultadoAdmin.Mensaje}",
                        null,
                        null
                    );
                }

                return new Response<Administrador>(
                    true,
                    "Administrador registrado exitosamente",
                    administrador,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }


        public Response<Administrador> ActualizarPermisos(int idAdmin, string nuevosPermisos)
        {
            try
            {
                if (idAdmin <= 0)
                {
                    return new Response<Administrador>(false, "ID inválido", null, null);
                }

                if (string.IsNullOrWhiteSpace(nuevosPermisos))
                {
                    return new Response<Administrador>(false, "Los permisos son requeridos", null, null);
                }

                var adminResponse = _administradorRepository.ObtenerPorId(idAdmin);
                if (!adminResponse.Estado)
                {
                    return new Response<Administrador>(false, "Administrador no encontrado", null, null);
                }

                var admin = adminResponse.Entidad;
                admin.Permisos = nuevosPermisos;

                return _administradorRepository.Actualizar(admin);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }


        public bool EsAdministrador(int idUsuario)
        {
            try
            {
                var todosAdmins = _administradorRepository.ObtenerTodos();

                if (!todosAdmins.Estado || todosAdmins.Lista == null)
                {
                    return false;
                }

                foreach (var admin in todosAdmins.Lista)
                {
                    if (admin.IdUsuario == idUsuario && admin.Estado == "Activo")
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un administrador tiene un permiso específico.
        /// </summary>
        public bool TienePermiso(int idAdmin, string permiso)
        {
            try
            {
                var adminResponse = _administradorRepository.ObtenerPorId(idAdmin);

                if (!adminResponse.Estado)
                {
                    return false;
                }

                var admin = adminResponse.Entidad;
                return admin.Permisos.Contains(permiso);
            }
            catch
            {
                return false;
            }
        }

        public Response<Administrador> ObtenerAdministradorPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Administrador>(false, "ID inválido", null, null);
                }

                return _administradorRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> ObtenerTodosLosAdministradores()
        {
            try
            {
                return _administradorRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}
