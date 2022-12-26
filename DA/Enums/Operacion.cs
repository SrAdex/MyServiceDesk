using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DA.Enums
{
    public enum Operacion
    {
        /*Ticket*/
        ListadoTickets = 1,
        MisTickets = 2,
        AsignarTicket = 3,
        ActualizarEstadoTicket = 4,
        ReporteGeneralTickets = 5,
        ReportePersonalTickets = 6,

        /*Usuario*/
        ListadoUsuarios = 7,

        /*Categoría*/
        ListadoCategorias = 8
    }
}