using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string CorreoUsuario { get; set; }
        public bool EsActivo { get; set; }
    }
}
