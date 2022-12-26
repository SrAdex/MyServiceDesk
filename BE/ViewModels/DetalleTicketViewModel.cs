using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.ViewModels
{
    public class DetalleTicketViewModel
    {
        public int IdTicket { get; set; }
        public string AsuntoTicket { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string UsuarioResponsable { get; set; }
        public DateTime? FechaAsignación { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string EstadoTicket { get; set; }
        public string CategoriaTicket { get; set; }
        public string SubCategoriaTicket { get; set; }
        public string PropietarioTicket { get; set; }
        public string GuidTicket { get; set; }
        //Agregados
        public int numeroDias { get; set; }
        public string tema { get; set; }
        public int tipoDeContenido { get; set; }
        public int numeroPiezas { get; set; }
        public string descripcion { get; set; }
        public string etapa { get; set; }
        public int numeroDiasVencido { get; set; }
        public string prioridad { get; set; }
        public string asignadoPor { get; set; }
        public int asignadoPorInt { get; set; }
        public DateTime? fechaEntrega { get; set; }
        //
        public string GetFechaFormateada(DateTime? fecha)
        {
            if (fecha != null)
            {
                return fecha.Value.ToString("dd MMM , yyyy hh:mm tt");
            }
            else
            {
                return "-";
            }
        }
    }
}
