using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEEncuesta
    {
        public int IdEncuesta { get; set; }
        public int IdTicket { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Satisfaccion { get; set; }
        public string Comentario { get; set; }
        public string Asunto { get; set; }
    }
}
