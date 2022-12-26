using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BECategoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public bool EsActivo { get; set; }
        public int IdCategoriaSuperior { get; set; }
    }
}
