using BE.ViewModels;
using BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DA
{
    public class DAUsuario
    {
        private readonly BEUsuario _usuarioSesion;

        public DAUsuario()
        {
            _usuarioSesion = HttpContext.Current.Session["usuario"] != null ? HttpContext.Current.Session["usuario"] as BEUsuario : new BEUsuario();
        }

        // Métodos verificados
        public BEUsuario BuscarUsuario(int idUsuario)
        {
            BEUsuario usuario = null;

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_BUSCAR_USUARIO", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    usuario = new BEUsuario()
                    {
                        IdUsuario = dr.GetInt32(0),
                        NombresUsuario = dr.GetString(1),
                        ApellidosUsuario = dr.GetString(2),
                        CorreoUsuario = dr.GetString(3),
                        EsActivo = dr.GetBoolean(4),
                        InicialesUsuario = dr.GetString(5)
                    };

                    usuario.RolesUsuario = ListarRolesUsuario(usuario.IdUsuario);
                }

                dr.Close();
                con.Close();
            }

            return usuario;
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

            return usuarios;
        }

        public int GetNuevoID()
        {
            int nuevoId = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("select max(id_usuario) from Usuario", con);
                con.Open();

                nuevoId = (int)cmd.ExecuteScalar();

                con.Close();
            }

            return (nuevoId + 1);
        }

        public string RegistrarUsuario(BEUsuario usuario)
        {
            string mensaje = "";
            int nuevoID = GetNuevoID();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlTransaction tr = con.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    SqlCommand cmd = new SqlCommand("USP_REGISTRA_USUARIO", con, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario_creacion", _usuarioSesion.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombres", usuario.NombresUsuario);
                    cmd.Parameters.AddWithValue("@apellidos", usuario.ApellidosUsuario);
                    cmd.Parameters.AddWithValue("@correo", usuario.CorreoUsuario);
                    cmd.Parameters.AddWithValue("@esActivo", usuario.EsActivo);
                    cmd.Parameters.AddWithValue("@iniciales", usuario.InicialesUsuario);

                    cmd.ExecuteNonQuery();

                    foreach (var rol in usuario.RolesUsuario)
                    {
                        cmd = new SqlCommand("USP_REGISTRA_USUARIO_ROL", con, tr);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idUsuario", nuevoID);
                        cmd.Parameters.AddWithValue("@idRol", rol.IdRol);

                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                    mensaje = "Usuario registrado correctamente";

                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    tr.Rollback();
                }
                finally
                {
                    con.Close();
                }
            }
            return mensaje;
        }

        public string EditarUsuario(BEUsuario usuario)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlTransaction tr = con.BeginTransaction(IsolationLevel.Serializable);

                try
                {
                    SqlCommand cmd = new SqlCommand("USP_EDITA_USUARIO", con, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", _usuarioSesion.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombres", usuario.NombresUsuario);
                    cmd.Parameters.AddWithValue("@apellidos", usuario.ApellidosUsuario);
                    cmd.Parameters.AddWithValue("@correo", usuario.CorreoUsuario);
                    cmd.Parameters.AddWithValue("@esActivo", usuario.EsActivo);
                    cmd.Parameters.AddWithValue("@iniciales", usuario.InicialesUsuario);
                    cmd.Parameters.AddWithValue("@idUsuario", usuario.IdUsuario);

                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("USP_RESETEA_ROL", con, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idUsuario", usuario.IdUsuario);
                    cmd.ExecuteNonQuery();


                    foreach (var rol in usuario.RolesUsuario)
                    {
                        cmd = new SqlCommand("USP_REGISTRA_USUARIO_ROL", con, tr);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idUsuario", usuario.IdUsuario);
                        cmd.Parameters.AddWithValue("@idRol", rol.IdRol);

                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                    mensaje = "Usuario editado correctamente";

                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                    tr.Rollback();
                }
                finally
                {
                    con.Close();
                }

            }

            return mensaje;
        }

        public string EliminarUsuario(int id)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Eliminar_Usuario", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", _usuarioSesion.IdUsuario);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Estado del Usuario actualizado a 'Eliminado'";
                }
                catch (Exception ex)
                {
                    mensaje = "Error : " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }

            return mensaje;
        }

        public List<BERol> ListarRolesUsuario(int idUsuario)
        {
            List<BERol> rolesUsuario = new List<BERol>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Listar_Roles_Usuario", con);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.CommandType = CommandType.StoredProcedure;


                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BERol rolUsuario = new BERol()
                    {
                        IdRol = dr.GetInt32(0),
                        NombreRol = dr.GetString(1)
                    };

                    rolesUsuario.Add(rolUsuario);
                }

                dr.Close();
                con.Close();
            }

            return rolesUsuario;
        }

        public List<UsuarioViewModel> ListarUsuariosRol(int idRol)
        {
            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Usuarios_Rol", con);
                cmd.Parameters.AddWithValue("@id_rol", idRol);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UsuarioViewModel usuario = new UsuarioViewModel()
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

            return usuarios;
        }
    }
}
