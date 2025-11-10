using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    internal class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        public Response<Usuario> RegistrarUsuario(Usuario usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return new Response<Usuario>(
                        false,
                        "El usuario no puede ser nulo",
                        null,
                        null
                    );
                }

                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    return new Response<Usuario>(false, "El nombre es obligatorio", null, null);
                }

                if (string.IsNullOrWhiteSpace(usuario.Correo))
                {
                    return new Response<Usuario>(false, "El correo es obligatorio", null, null);
                }

                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    return new Response<Usuario>(false, "La contraseña es obligatoria", null, null);
                }


                if (!EsEmailValido(usuario.Correo))
                {
                    return new Response<Usuario>(
                        false,
                        "El formato del correo no es válido",
                        null,
                        null
                    );
                }

                var resultadoValidacionPassword = ValidarContraseñaSegura(usuario.Contrasena);
                if (!resultadoValidacionPassword.Estado)
                {
                    return resultadoValidacionPassword;
                }

                var usuarioExistente = ObtenerPorCorreo(usuario.Correo);
                if (usuarioExistente.Estado && usuarioExistente.Entidad != null)
                {
                    return new Response<Usuario>(
                        false,
                        "Ya existe un usuario con ese correo electrónico",
                        null,
                        null
                    );
                }

                if (string.IsNullOrWhiteSpace(usuario.Estado))
                {
                    usuario.Estado = "Activo";
                }

                var resultado = _usuarioRepository.Insertar(usuario);

                return resultado;
            }
            catch (Exception ex)
            {

                return new Response<Usuario>(
                    false,
                    $"Error al registrar usuario: {ex.Message}",
                    null,
                    null
                );
            }
        }


        public Response<Usuario> ActualizarUsuario(Usuario usuario)
        {
            try
            {

                if (usuario == null || usuario.IdUsuario <= 0)
                {
                    return new Response<Usuario>(false, "Datos inválidos para actualizar", null, null);
                }


                var usuarioExistente = _usuarioRepository.ObtenerPorId(usuario.IdUsuario);
                if (!usuarioExistente.Estado)
                {
                    return new Response<Usuario>(false, "El usuario no existe", null, null);
                }

                if (!string.IsNullOrWhiteSpace(usuario.Correo) && !EsEmailValido(usuario.Correo))
                {
                    return new Response<Usuario>(false, "El formato del correo no es válido", null, null);
                }


                return _usuarioRepository.Actualizar(usuario);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al actualizar: {ex.Message}", null, null);
            }
        }


        public Response<Usuario> DesactivarUsuario(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    return new Response<Usuario>(false, "ID inválido", null, null);
                }


                var usuario = _usuarioRepository.ObtenerPorId(idUsuario);
                if (!usuario.Estado)
                {
                    return new Response<Usuario>(false, "Usuario no encontrado", null, null);
                }


                return _usuarioRepository.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error: {ex.Message}", null, null);
            }
        }


        public Response<Usuario> ObtenerUsuarioPorId(int idUsuario)
        {
            try
            {
                if (idUsuario <= 0)
                {
                    return new Response<Usuario>(false, "ID inválido", null, null);
                }

                return _usuarioRepository.ObtenerPorId(idUsuario);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Usuario> ObtenerTodosLosUsuarios()
        {
            try
            {
                return _usuarioRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error: {ex.Message}", null, null);
            }
        }


        public Response<Usuario> ValidarCredenciales(string correo, string contrasena)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
                {
                    return new Response<Usuario>(false, "Correo y contraseña son requeridos", null, null);
                }


                var usuarioResponse = ObtenerPorCorreo(correo);
                if (!usuarioResponse.Estado || usuarioResponse.Entidad == null)
                {
                    return new Response<Usuario>(false, "Credenciales incorrectas", null, null);
                }

                var usuario = usuarioResponse.Entidad;


                if (usuario.Estado != "Activo")
                {
                    return new Response<Usuario>(false, "Usuario inactivo", null, null);
                }


                if (usuario.Contrasena != contrasena)
                {
                    return new Response<Usuario>(false, "Credenciales incorrectas", null, null);
                }

                return new Response<Usuario>(true, "Login exitoso", usuario, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error en validación: {ex.Message}", null, null);
            }
        }


        private Response<Usuario> ObtenerPorCorreo(string correo)
        {
            try
            {
                var todosLosUsuarios = _usuarioRepository.ObtenerTodos();

                if (!todosLosUsuarios.Estado || todosLosUsuarios.Lista == null)
                {
                    return new Response<Usuario>(false, "No se pudo consultar usuarios", null, null);
                }

                foreach (var usuario in todosLosUsuarios.Lista)
                {
                    if (usuario.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase))
                    {
                        return new Response<Usuario>(true, "Usuario encontrado", usuario, null);
                    }
                }

                return new Response<Usuario>(false, "Usuario no encontrado", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error: {ex.Message}", null, null);
            }
        }


        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {

                string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, patron, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }


        private Response<Usuario> ValidarContraseñaSegura(string contrasena)
        {
            if (contrasena.Length < 8)
            {
                return new Response<Usuario>(
                    false,
                    "La contraseña debe tener al menos 8 caracteres",
                    null,
                    null
                );
            }

            if (!Regex.IsMatch(contrasena, @"[A-Z]"))
            {
                return new Response<Usuario>(
                    false,
                    "La contraseña debe contener al menos una letra mayúscula",
                    null,
                    null
                );
            }


            if (!Regex.IsMatch(contrasena, @"[0-9]"))
            {
                return new Response<Usuario>(
                    false,
                    "La contraseña debe contener al menos un número",
                    null,
                    null
                );
            }

            return new Response<Usuario>(true, "Contraseña válida", null, null);
        }
    }
}

