using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BEUsuario
    {
        public int IdUsuario { get; set; }
        public string NombresUsuario { get; set; }
        public string ApellidosUsuario { get; set; }
        public string CorreoUsuario { get; set; }
        public bool EsActivo { get; set; }
        public List<BERol> RolesUsuario { get; set; }
        public string InicialesUsuario { get; set; }

        public string NombreCompleto { get { return NombresUsuario + ' ' + ApellidosUsuario; } }
        public string Nom { get { return NombresUsuario; } }
        public string Ape { get { return ApellidosUsuario; } }
    }
}
