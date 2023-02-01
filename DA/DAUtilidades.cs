using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BE.ViewModels;

namespace DA
{
    public class DAUtilidades
    {
        public List<BECategoria> ListarCategorias()
        {
            List<BECategoria> categorias = new List<BECategoria>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Categorias", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BECategoria categoria = new BECategoria()
                    {
                        IdCategoria = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        EsActivo = dr.GetBoolean(2)
                    };

                    categorias.Add(categoria);
                }

                dr.Close();
                con.Close();
            }

            return categorias;
        }

        public List<BEEstado> ListarEstados()
        {
            List<BEEstado> estados = new List<BEEstado>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_ESTADOS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEEstado estado = new BEEstado
                    {
                        IdEstado = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        EsActivo = dr.GetBoolean(2)
                    };
                    estados.Add(estado);
                }

                var usuario = HttpContext.Current.Session["usuario"] as BEUsuario ?? new BEUsuario();
                int indexRolAdmin = usuario.RolesUsuario.FindIndex(r => r.NombreRol == "Administrador");
                int indexEstadoEliminado = estados.FindIndex(e => e.Nombre == "Eliminado");

                // Si no tiene el rol de administrador
                if (indexRolAdmin < 0)
                {
                    // Eliminar el estado eliminado
                    estados.RemoveAt(indexEstadoEliminado);
                }

                dr.Close();
                con.Close();
            }

            return estados;
        }

        public List<BEAdjunto> ListarAdjuntosCorreo(string codigoCorreo)
        {
            List<BEAdjunto> adjuntos = new List<BEAdjunto>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Adjuntos_Correo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", codigoCorreo);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEAdjunto adjunto = new BEAdjunto()
                    {
                        RutaAdjunto = dr.GetString(0),
                        NombreAdjunto = dr.GetString(1),
                        CidAdjunto = dr.GetString(2)
                    };

                    adjuntos.Add(adjunto);
                }

                dr.Close(); con.Close();
            }
            return adjuntos;
        }

        public List<BECategoria> ListarSubCategorias(int idCategoriaSuperior)
        {
            List<BECategoria> categorias = new List<BECategoria>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Subcategorias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_categoria_superior", idCategoriaSuperior);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BECategoria categoria = new BECategoria()
                    {
                        IdCategoria = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        EsActivo = dr.GetBoolean(2),
                        IdCategoriaSuperior = dr.GetInt32(3)
                    };

                    categorias.Add(categoria);
                }

                dr.Close();
                con.Close();
            }
            return categorias;
        }

        public List<BEDireccionCorreo> ListarDireccionesCorreo(string codigoCorreo, int tipoRelacion)
        {
            List<BEDireccionCorreo> direccionesCorreo = new List<BEDireccionCorreo>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Direcciones_Correo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", codigoCorreo);
                cmd.Parameters.AddWithValue("@tipo_relacion", tipoRelacion);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEDireccionCorreo direccion = new BEDireccionCorreo
                    {
                        UsuarioCorreo = dr.GetString(0),
                        Direccion = dr.GetString(1)
                    };

                    direccionesCorreo.Add(direccion);
                }

                dr.Close(); con.Close();
            }

            return direccionesCorreo;
        }

        public List<BE.TDCxTicket> ListarTipoDeContenidoPorID()
        {
            List<BE.TDCxTicket> TDCxTickets = new List<BE.TDCxTicket>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_TipoDeContenido_por_IdTicket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.TDCxTicket TDCxTicket = new BE.TDCxTicket
                    {
                        id_ticket = dr.GetInt32(0),
                        id_tipoDeContenido = dr.GetInt32(1),
                        tipocontenidoDesc = dr.GetString(2)
                    };

                    TDCxTickets.Add(TDCxTicket);
                }


                dr.Close();
                con.Close();
            }

            return TDCxTickets;
        }

        public List<BE.BETipoDeContenido> ListarTipoDeContenidos()
        {
            List<BE.BETipoDeContenido> TipoDeContenidos = new List<BE.BETipoDeContenido>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_TipoDeContenido", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.BETipoDeContenido tipoDeContenido = new BE.BETipoDeContenido
                    {
                        id_tipocontenido = dr.GetInt32(0),
                        descripcion = dr.GetString(1)
                    };

                    TipoDeContenidos.Add(tipoDeContenido);
                }
                dr.Close();
                con.Close();
            }

            return TipoDeContenidos;
        }

        public IEnumerable<UsuarioViewModel> ListarUsuarios()
        {
            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_USUARIOS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UsuarioViewModel usuario = new UsuarioViewModel
                    {
                        IdUsuario = dr.GetInt32(0),
                        NombreCompleto = dr.GetString(1),
                        CorreoUsuario = dr.GetString(2),
                        EsActivo = dr.GetBoolean(3)
                    };

                    usuarios.Add(usuario);
                }

                dr.Close();
                con.Close();
            }

            UsuarioViewModel defaultUsuario = new UsuarioViewModel() { IdUsuario = 0, NombreCompleto = "--Seleccione--", EsActivo = true };

            usuarios.Insert(0, defaultUsuario);

            return usuarios;
        }
    }
}
