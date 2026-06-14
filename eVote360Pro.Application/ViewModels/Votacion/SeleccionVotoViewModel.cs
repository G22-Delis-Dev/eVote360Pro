using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360Pro.Application.ViewModels.Votacion;

    public class SeleccionVotoViewModel
    {
        public int PuestoElectivoId { get; set; }

        // Nulos en caso de que el ciudadano vote en blanco en ese puesto
        public int? CandidatoId { get; set; }
        public int? PartidoPoliticoId { get; set; }
    }