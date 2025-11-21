using DAL;
using ENTITY;
using System;

namespace BLL
{
    public class ReclutadorService
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
                if (reclutador == null) return new Response<Reclutador>(false, "El reclutador no puede ser nulo", null, null);
                if (reclutador.IdEmpresa <= 0) return new Response<Reclutador>(false, "Debe especificar una empresa válida", null, null);

                var empresaResponse = _empresaRepository.ObtenerPorId(reclutador.IdEmpresa);
                if (!empresaResponse.Estado) return new Response<Reclutador>(false, "La empresa especificada no existe", null, null);

                // 1. Crear Usuario Base
                // NOTA: Asignamos Foto como null explícitamente si no viene, para evitar problemas
                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = reclutador.Nombre,
                    Correo = reclutador.Correo,
                    Contrasena = reclutador.Contrasena,
                    Estado = "Activo",
                    TipoUsuario = "Reclutador",
                    Foto = null // Por ahora el reclutador no sube foto en el registro simple
                };

                var resultadoUsuario = _usuarioService.RegistrarUsuario(nuevoUsuario);

                if (!resultadoUsuario.Estado)
                {
                    return new Response<Reclutador>(false, resultadoUsuario.Mensaje, null, null);
                }

                // 2. Asignar el ID generado al Reclutador
                reclutador.IdUsuario = resultadoUsuario.Entidad.IdUsuario;
                // Aseguramos que el ID de la entidad hija sea igual al del padre
                reclutador.IdReclutador = reclutador.IdUsuario;

                var resultadoReclutador = _reclutadorRepository.Insertar(reclutador);

                if (!resultadoReclutador.Estado)
                {
                    return new Response<Reclutador>(false, $"Usuario creado pero error al guardar reclutador: {resultadoReclutador.Mensaje}", null, null);
                }

                return new Response<Reclutador>(true, "Reclutador registrado exitosamente", reclutador, null);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // ... (El resto de métodos se mantienen igual: Obtener, Actualizar, etc.) ...
        public Response<Reclutador> ObtenerReclutadorPorId(int id) => _reclutadorRepository.ObtenerPorId(id);
        public Response<Reclutador> ActualizarReclutador(Reclutador reclutador) => _reclutadorRepository.Actualizar(reclutador);
        public Response<Reclutador> ObtenerTodosLosReclutadores() => _reclutadorRepository.ObtenerTodos();
    }
}