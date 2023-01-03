using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA
{
    public class DADashboard
    {
        private readonly DAUsuario gestionUsuario;
        private readonly DAEstado gestionEstado;
        private readonly BE.BEUsuario usuario;

        public DADashboard()
        {
            gestionUsuario = new DAUsuario();
            gestionEstado = new DAEstado();
            //usuario = HttpContext.Current.Session["usuario"] != null ? HttpContext.Current.Session["usuario"] as ClassLibraryBE.Usuario : new ClassLibraryBE.Usuario();
        }

        public List<BE.BEDashboard> Dashboard_TicketsXFechaGen()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXFechaGen = new List<BE.BEDashboard>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TicketsXFechaGen", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.BEDashboard dash = new BE.BEDashboard()
                    {
                        Dashboard_TicketsXFechaGen_Anio = Convert.ToString(dr.GetInt32(0)),
                        Dashboard_TicketsXFechaGen_MesNum = Convert.ToString(dr.GetInt32(1)),
                        Dashboard_TicketsXFechaGen_MesName = dr.GetString(2),
                        Dashboard_TicketsXFechaGen_Count = Convert.ToString(dr.GetInt32(3)),
                    };
                    listaDashboard_TicketsXFechaGen.Add(dash);

                }

                dr.Close();
                con.Close();
            }

            return listaDashboard_TicketsXFechaGen;
        }

        public List<BE.BEDashboard> Dashboard_TicketsXEstado()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXEstado = new List<BE.BEDashboard>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TicketsXEstado", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.BEDashboard dash = new BE.BEDashboard()
                    {
                        Dashboard_TicketsXEstado_ID = Convert.ToString(dr.GetInt32(0)),
                        Dashboard_TicketsXEstado_EstadoName = dr.GetString(1),
                        Dashboard_TicketsXEstado_Count = Convert.ToString(dr.GetInt32(2)),
                    };
                    listaDashboard_TicketsXEstado.Add(dash);

                }

                dr.Close();
                con.Close();
            }

            return listaDashboard_TicketsXEstado;
        }

        public List<BE.BEDashboard> Dashboard_TicketsXUsuario()
        {
            List<BE.BEDashboard> listaDashboard_TicketsXEstado = new List<BE.BEDashboard>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TicketsXUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.BEDashboard dash = new BE.BEDashboard()
                    {
                        Dashboard_TicketsXUsuario_IdUsuario = Convert.ToString(dr.GetInt32(0)),
                        Dashboard_TicketsXUsuario_NombreCompleto = dr.GetString(1),
                        Dashboard_TicketsXUsuario_IdEstado = Convert.ToString(dr.GetInt32(2)),
                        Dashboard_TicketsXUsuario_NombreEstado = dr.GetString(3),
                        Dashboard_TicketsXUsuario_Count = Convert.ToString(dr.GetInt32(4)),
                    };
                    listaDashboard_TicketsXEstado.Add(dash);

                }

                dr.Close();
                con.Close();
            }

            return listaDashboard_TicketsXEstado;
        }

        public int DashboardTotalDeTickets()
        {
            int count = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TotalTickets", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    count = dr.GetInt32(0);
                }

                dr.Close();
                con.Close();
            }
            return count;
        }

        public int DashboardTotalEncuestas()
        {
            int count = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TotalEncuestas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    count = dr.GetInt32(0);
                }

                dr.Close();
                con.Close();
            }
            return count;
        }

        public int DashboardTotalTicketsRecibidos()
        {
            int count = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TotalTicketsRecibidos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    count = dr.GetInt32(0);
                }

                dr.Close();
                con.Close();
            }
            return count;
        }

        public int DashboardTotalUsuariosActivos()
        {
            int count = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Dashboard_TotalUsuariosActivos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    count = dr.GetInt32(0);
                }

                dr.Close();
                con.Close();
            }
            return count;
        }
        //public List<ClassLibraryBE.Dashboard> Dashboard_TicketsXUsuario2()
        //{
        //    List<ClassLibraryBE.Dashboard> lst = new List<ClassLibraryBE.Dashboard>();
        //    ClassLibraryBE.Dashboard dash = new ClassLibraryBE.Dashboard();
        //    //
        //    List<ClassLibraryBE.Dashboard> lstDashboard_TicketsXUsuario = new List<ClassLibraryBE.Dashboard>();
        //    lstDashboard_TicketsXUsuario = Dashboard_TicketsXUsuario();
        //    List<ClassLibraryBE.ViewModels.UsuarioViewModel> lstUsuarios = new List<ClassLibraryBE.ViewModels.UsuarioViewModel>();
        //    lstUsuarios = gestionUsuario.DashListarUsuarios();
        //    List<ClassLibraryBE.Estado> lstEstados = new List<ClassLibraryBE.Estado>();
        //    lstEstados = gestionEstado.GetListaEstados();
        //    foreach(var itemDashTxU in lstDashboard_TicketsXUsuario)
        //    {
        //        foreach(var itemUsuario in lstUsuarios)
        //        {
        //            if(Convert.ToInt32(itemUsuario.IdUsuario) == Convert.ToInt32(itemDashTxU.Dashboard_TicketsXUsuario_IdUsuario))
        //            {
        //                //foreach(var itemEstado in lstEstados)
        //                //{
        //                    int idEstadoDashUsu = Convert.ToInt32(itemDashTxU.Dashboard_TicketsXUsuario_IdEstado);
        //                    if (idEstadoDashUsu == 1|| idEstadoDashUsu == 2 || idEstadoDashUsu == 3 || idEstadoDashUsu == 4 || idEstadoDashUsu == 5 || idEstadoDashUsu == 6)
        //                    {
        //                        dash.Dashboard_TicketsXUsuario_IdUsuario = itemDashTxU.Dashboard_TicketsXUsuario_IdUsuario;
        //                        dash.Dashboard_TicketsXUsuario_NombreCompleto = itemDashTxU.Dashboard_TicketsXUsuario_NombreCompleto;
        //                        dash.Dashboard_TicketsXUsuario_IdEstado = itemDashTxU.Dashboard_TicketsXUsuario_IdEstado;
        //                        dash.Dashboard_TicketsXUsuario_NombreEstado = itemDashTxU.Dashboard_TicketsXUsuario_NombreEstado;
        //                        dash.Dashboard_TicketsXUsuario_Count = itemDashTxU.Dashboard_TicketsXUsuario_Count;
        //                        lst.Add(dash);
        //                    }
        //                    else
        //                    {
        //                        dash.Dashboard_TicketsXUsuario_IdUsuario = itemDashTxU.Dashboard_TicketsXUsuario_IdUsuario;
        //                        dash.Dashboard_TicketsXUsuario_NombreCompleto = itemDashTxU.Dashboard_TicketsXUsuario_NombreCompleto;
        //                        foreach (var itemEstado in lstEstados)
        //                        {
        //                            if(itemEstado.IdEstado==idEstadoDashUsu)
        //                            {
        //                            dash.Dashboard_TicketsXUsuario_IdEstado = Convert.ToString(itemEstado.IdEstado);
        //                            dash.Dashboard_TicketsXUsuario_NombreEstado = itemEstado.Nombre;
        //                            }
        //                        }

        //                        dash.Dashboard_TicketsXUsuario_Count = "0";
        //                        lst.Add(dash);
        //                    }
        //                }
        //            //}
        //            else
        //            {
        //                //foreach(var itemEstado2 in lstEstados)
        //                //{
        //                //    dash.Dashboard_TicketsXUsuario_IdUsuario = Convert.ToString(itemUsuario.IdUsuario);
        //                //    dash.Dashboard_TicketsXUsuario_NombreCompleto = itemUsuario.NombreCompleto;
        //                //    dash.Dashboard_TicketsXUsuario_IdEstado = Convert.ToString(itemEstado2.IdEstado);
        //                //    dash.Dashboard_TicketsXUsuario_NombreEstado = itemEstado2.Nombre;
        //                //    dash.Dashboard_TicketsXUsuario_Count = "0";
        //                //    lst.Add(dash);
        //                //}
        //            }
        //        }
        //    }

        //    return lst;
        //}
    }
}
