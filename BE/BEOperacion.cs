using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEOperacion
    {
        public int IdOperacion { get; set; }
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public int IdModulo { get; set; }
        public bool MostrarEnMenu { get; set; }

        public string GetAction()
        {
            int slashAction = Ruta.LastIndexOf("/") + 1;
            return Ruta.Substring(slashAction, Ruta.Length - slashAction);
        }
    }
}
