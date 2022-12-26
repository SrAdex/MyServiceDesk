using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.ViewModels
{
    public class TicketViewModel
    {
        public int IdTicket { get; set; }
        public string CodigoTicket { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaGene { get; set; }
        public string AsuntoTicket { get; set; }
        public string EstadoTicket { get; set; }
        public string UsuarioResponsable { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string InicialesResponsable { get; set; }
        //Agregados
        public int numeroDias { get; set; }
        public string tema { get; set; }
        public int tipoDeContenido { get; set; }
        public int numeroPiezas { get; set; }
        public string descripcion { get; set; }
        public string etapa { get; set; }
        public int numeroDiasVencido { get; set; }
        public string prioridad { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public bool vencido { get; set; }
        public string nombreusuario { get; set; }
        public string apellidosusuario { get; set; }


        public int diasEnMKTG { get; set; }
        public int diasVencido { get; set; }



        public string GetFechaFormateada(string tipoFormato, DateTime? fecha)
        {
            if (fecha != null)
            {
                if (tipoFormato == "tabla")
                {
                    return fecha.Value.ToString("dd/MM/yy - HH:mm");
                }
                else
                {
                    return fecha.Value.ToString("yyyy MM dd HH mm ss fff");
                }
            }
            else
            {
                return "-";
            }
        }
    }
}
