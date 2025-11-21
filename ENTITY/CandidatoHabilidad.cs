using System;

namespace ENTITY
{
    public class CandidatoHabilidad
    {
        public int IdCandidatoHabilidad { get; set; } // PK
        public int IdCandidato { get; set; }          // FK Candidato
        public int IdHabilidad { get; set; }          // FK Habilidad
        public string NivelDominio { get; set; }      // varchar(50)

        public CandidatoHabilidad() { }

        public CandidatoHabilidad(
            int idCandidatoHabilidad,
            int idCandidato,
            int idHabilidad,
            string nivelDominio
        )
        {
            IdCandidatoHabilidad = idCandidatoHabilidad;
            IdCandidato = idCandidato;
            IdHabilidad = idHabilidad;
            NivelDominio = nivelDominio;
        }
    }
}
