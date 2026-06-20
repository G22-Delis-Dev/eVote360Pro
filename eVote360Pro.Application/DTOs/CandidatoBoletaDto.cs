using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360Pro.Application.DTOs
{
    public class CandidatoBoletaDto
    {
        public int CandidatoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public int PartidoPoliticoId { get; set; }
        public string PartidoNombre { get; set; } = string.Empty;
        public string LogoPartido { get; set; } = string.Empty;
    }
}
