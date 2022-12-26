using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using BE;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace DA
{
    public class DACategoria
    {
        private readonly BEUsuario usuario;

        public DACategoria()
        {
            usuario = HttpContext.Current.Session["usuario"] != null ? HttpContext.Current.Session["usuario"] as BEUsuario : new BEUsuario();
        }

        public string RegistrarCategoria(BECategoria categoria)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Registrar_Categoria", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario_creacion", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                    cmd.Parameters.AddWithValue("@esActivo", categoria.EsActivo);

                    con.Open();
                    int rs = cmd.ExecuteNonQuery();

                    mensaje = "Categoría registrada correctamente";
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return mensaje;
        }

        public string RegistrarSubCategoria(BECategoria subcategoria)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Registrar_Subcategoria", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario_creacion", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@nombre", subcategoria.Nombre);
                    cmd.Parameters.AddWithValue("@esActivo", subcategoria.EsActivo);
                    cmd.Parameters.AddWithValue("@idCategoriaSuperior", subcategoria.IdCategoriaSuperior);

                    con.Open();
                    int rs = cmd.ExecuteNonQuery();

                    mensaje = "Subcategoría registrada correctamente";
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("FK"))
                    {
                        mensaje = "Error: Para registrar una subcategoría debe seleccionar una categoría.";
                    }
                    mensaje = "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return mensaje;
        }

        public string EditarCategoria(BECategoria categoria)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Editar_Categoria", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                    cmd.Parameters.AddWithValue("@esActivo", categoria.EsActivo);
                    cmd.Parameters.AddWithValue("@idCategoria", categoria.IdCategoria);
                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);

                    con.Open();
                    int rs = cmd.ExecuteNonQuery();

                    mensaje = "Categoría/Subcategoría editada correctamente";
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return mensaje;
        }

        public string EliminarCategoria(int idCategoria)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Eliminar_Categoria", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);

                    con.Open();
                    int rs = cmd.ExecuteNonQuery();

                    mensaje = "Categoría/Subcategoría eliminada correctamente";
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
            return mensaje;
        }
    }
}
