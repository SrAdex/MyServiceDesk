using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEDashboard
    {
        public string Dashboard_TicketsXFechaGen_Anio { get; set; }
        public string Dashboard_TicketsXFechaGen_MesNum { get; set; }
        public string Dashboard_TicketsXFechaGen_MesName { get; set; }
        public string Dashboard_TicketsXFechaGen_Count { get; set; }
        //
        public string Dashboard_TicketsXEstado_ID { get; set; }
        public string Dashboard_TicketsXEstado_EstadoName { get; set; }
        public string Dashboard_TicketsXEstado_Count { get; set; }
        //
        public string Dashboard_TicketsXUsuario_IdUsuario { get; set; }
        public string Dashboard_TicketsXUsuario_NombreCompleto { get; set; }
        public string Dashboard_TicketsXUsuario_IdEstado { get; set; }
        public string Dashboard_TicketsXUsuario_NombreEstado { get; set; }
        public string Dashboard_TicketsXUsuario_Count { get; set; }
    }
}
