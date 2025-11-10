using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    internal class EmpresaService
    {
        private readonly EmpresaRepository _empresaRepository;

        public EmpresaService()
        {
            _empresaRepository = new EmpresaRepository();
        }

        public Response<Empresa> RegistrarEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa == null)
                {
                    return new Response<Empresa>(false, "La empresa no puede ser nula", null, null);
                }

                if (string.IsNullOrWhiteSpace(empresa.Nombre))
                {
                    return new Response<Empresa>(false, "El nombre es obligatorio", null, null);
                }

                if (string.IsNullOrWhiteSpace(empresa.CorreoContacto))
                {
                    return new Response<Empresa>(false, "El correo de contacto es obligatorio", null, null);
                }

                // Validar formato de email
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    empresa.CorreoContacto,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return new Response<Empresa>(false, "Formato de correo inválido", null, null);
                }

                return _empresaRepository.Insertar(empresa);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ActualizarEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa == null || empresa.IdEmpresa <= 0)
                {
                    return new Response<Empresa>(false, "Datos inválidos", null, null);
                }

                return _empresaRepository.Actualizar(empresa);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerEmpresaPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Empresa>(false, "ID inválido", null, null);
                }

                return _empresaRepository.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerTodasLasEmpresas()
        {
            try
            {
                return _empresaRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> EliminarEmpresa(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Empresa>(false, "ID inválido", null, null);
                }

                return _empresaRepository.Eliminar(id);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}
