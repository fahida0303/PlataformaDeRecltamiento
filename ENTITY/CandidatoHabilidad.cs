using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class CandidatoHabilidad
    {
        public int IdCandidatoHabilidad { get; set; }
        public int IdCandidato { get; set; }
        public int IdHabilidad { get; set; }
        public string NivelDominio { get; set; }

        public CandidatoHabilidad() { }

        public CandidatoHabilidad(int idCandidatoHabilidad, int idCandidato, int idHabilidad, string nivelDominio)
        {
            IdCandidatoHabilidad = idCandidatoHabilidad;
            IdCandidato = idCandidato;
            IdHabilidad = idHabilidad;
            NivelDominio = nivelDominio;
        }
    }
}
