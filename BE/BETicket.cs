using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BETicket
    {
        public int IdTicket { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FecAsig { get; set; }
        public DateTime FechaCierre { get; set; }
        public string AsuntoTicket { get; set; }
        public int IdEstado { get; set; }
        public int IdUsuarioResponsable { get; set; }
        public int IdCategoria { get; set; }
        public int IdSubcategoria { get; set; }
        public string SolucionTicket { get; set; }
        public string Propietario { get; set; }
        //// Datos Agregados
        public int numeroDias { get; set; }
        public string tema { get; set; }
        public int tipoDeContenido { get; set; }
        public int numeroPiezas { get; set; }
        public string descripcion { get; set; }
        public int etapa { get; set; }
        public int numeroDiasVencido { get; set; }
        public DateTime fechaEntrega { get; set; }
        //public List<TDC_Ticket> TDCxT { get; set; }

        //// Datos de relación
        public string NombreEstado { get; set; }
        public string UsuarioResponsable { get; set; }
        public string Asunto { get; set; }
        public string NombreSubcategoria { get; set; }
    }
}
