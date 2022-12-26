using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BE;


namespace DA
{
    public class DAAccount
    {
        public bool EstaEnActiveDirectory(string correo, string clave)
        {
            bool resultado = false;
            string dominio = ConfigurationManager.AppSettings["DomainActiveDirectory"];
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, dominio))
            {
                resultado = pc.ValidateCredentials(correo, clave);
            }

            return resultado;
        }
        

        public BEUsuario EstaEnBaseDeDatos(string correo)
        {
            BEUsuario usuario = null;

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Validar_Acceso", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@correo_usuario", correo);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    usuario = new BEUsuario()
                    {
                        IdUsuario = dr.GetInt32(0),
                        NombresUsuario = dr.GetString(1),
                        ApellidosUsuario = dr.GetString(2),
                        CorreoUsuario = dr.GetString(3)
                    };
                }

                dr.Close();
                con.Close();
            }
            return usuario;
        }

        
        public void IniciarSesion(BEUsuario usuario)
        {
            HttpContext.Current.Session["usuario"] = usuario;
            HttpContext.Current.Session["filtroEstadoTicketGeneral"] = new List<int>() { 1 };
            HttpContext.Current.Session["filtroEstadoTicketPersonal"] = new List<int>() { 1 };
            HttpContext.Current.Session["filtroResponsableTicket"] = new List<int>() { 0 };
            HttpContext.Current.Session["minutosSesionRestantes"] = 560;
            HttpContext.Current.Session["modulos"] = GetModulosByRol(usuario.RolesUsuario.Max(r => r.IdRol));
            HttpContext.Current.Session["operaciones"] = GetOperacionesByRol(usuario.RolesUsuario.Max(r => r.IdRol));
            HttpContext.Current.Session.Timeout = 20;
            //
            bool rpt = usuario.RolesUsuario.FindIndex(r => r.NombreRol == "Administrador") >= 0;
            if (rpt)
            {
                HttpContext.Current.Session["adminOno"] = "Si";
            }
            else
            {
                HttpContext.Current.Session["adminOno"] = "No";
            }

        }

         public void CerrarSesion()
        {
            HttpContext.Current.Session.Abandon();
        }
        

        public List<BEModulo> GetModulosByRol(int idRol)
        {
            List<BEModulo> modulos = new List<BEModulo>();
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_listar_modulos_por_rol", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_rol", idRol);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    BEModulo modulo = new BEModulo()
                    {
                        IdModulo = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Icono = dr.GetString(2),
                        MostrarEnMenu = dr.GetBoolean(3)
                    };
                    if (modulo.IdModulo == 1)
                    {
                        modulo.collapse = "#collapseTwoo";
                    }
                    if (modulo.IdModulo == 2)
                    {
                        modulo.collapse = "#collapseTwooo";
                    }
                    if (modulo.IdModulo == 5)
                    {
                        modulo.collapse = "#collapseUtilities";
                    }
                    modulos.Add(modulo);
                }
                dr.Close();
                con.Close();
            }
            return modulos;
        }


        public List<BEOperacion> GetOperacionesByRol(int idRol)
        {
            List<BEOperacion> operaciones = new List<BEOperacion>();
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_listar_operaciones_por_rol", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_rol", idRol);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    BEOperacion operacion = new BEOperacion()
                    {
                        IdOperacion = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Ruta = dr.GetString(2),
                        IdModulo = dr.GetInt32(3),
                        MostrarEnMenu = dr.GetBoolean(4)
                    };

                    operaciones.Add(operacion);
                }
                dr.Close();
                con.Close();
            }
            return operaciones;
        }
    }
}
