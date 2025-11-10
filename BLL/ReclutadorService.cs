using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class ReclutadorService
    {
        private readonly ReclutadorRepository _reclutadorRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly EmpresaRepository _empresaRepository;
        private readonly UsuarioService _usuarioService;

        public ReclutadorService()
        {
            _reclutadorRepository = new ReclutadorRepository();
            _usuarioRepository = new UsuarioRepository();
            _empresaRepository = new EmpresaRepository();
            _usuarioService = new UsuarioService();
        }

   
        public Response<Reclutador> RegistrarReclutador(Reclutador reclutador)
        {
            try
            {
                if (reclutador == null)
                {
                    return new Response<Reclutador>(false, "El reclutador no puede ser nulo", null, null);
                }


                if (reclutador.IdEmpresa <= 0)
                {
                    return new Response<Reclutador>(false, "Debe especificar una empresa válida", null, null);
                }

                var empresaResponse = _empresaRepository.ObtenerPorId(reclutador.IdEmpresa);
                if (!empresaResponse.Estado)
                {
                    return new Response<Reclutador>(false, "La empresa especificada no existe", null, null);
                }


                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = reclutador.Nombre,
                    Correo = reclutador.Correo,
                    Contrasena = reclutador.Contrasena,
                    Estado = "Activo"
                };

                var resultadoUsuario = _usuarioService.RegistrarUsuario(nuevoUsuario);

                if (!resultadoUsuario.Estado)
                {
                    return new Response<Reclutador>(false, resultadoUsuario.Mensaje, null, null);
                }

                reclutador.IdUsuario = resultadoUsuario.Entidad.IdUsuario;

                var resultadoReclutador = _reclutadorRepository.Insertar(reclutador);

                if (!resultadoReclutador.Estado)
                {
                    return new Response<Reclutador>(
                        false,
                        $"Usuario creado pero error al guardar reclutador: {resultadoReclutador.Mensaje}",
                        null,
                        null
                    );
                }

                return new Response<Reclutador>(
                    true,
                    "Reclutador registrado exitosamente",
                    reclutador,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerReclutadorPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Reclutador>(false, "ID inválido", null, null);
                }

                return _reclutadorRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ActualizarReclutador(Reclutador reclutador)
        {
            try
            {
                if (reclutador == null || reclutador.IdUsuario <= 0)
                {
                    return new Response<Reclutador>(false, "Datos inválidos", null, null);
                }

                return _reclutadorRepository.Actualizar(reclutador);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerTodosLosReclutadores()
        {
            try
            {
                return _reclutadorRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

