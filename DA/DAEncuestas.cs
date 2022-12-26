using BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA
{
    public class DAEncuestas
    {
        public bool ExisteEncuesta(int idTicket)
        {
            bool existe;

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_existe_encuesta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                SqlDataReader dr = cmd.ExecuteReader();

                existe = dr.Read();

                dr.Close();
                con.Close();
            }

            return existe;
        }

        public BEEncuesta GetDeatlle(int idTicket)
        {
            BEEncuesta encuesta = new BEEncuesta() { Asunto = "-", Comentario = "-" };

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Detallar_Encuesta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    encuesta = new BEEncuesta()
                    {
                        Asunto = dr.GetString(0),
                        Satisfaccion = dr.GetInt32(1),
                        Comentario = dr.GetString(2)
                    };
                }

                dr.Close();
                con.Close();
            }

            return encuesta;
        }

        public string RegistrarEncuesta(BEEncuesta encuesta)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_registrar_encuesta", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_ticket", encuesta.IdTicket);
                    cmd.Parameters.AddWithValue("@satisfaccion", encuesta.Satisfaccion);
                    cmd.Parameters.AddWithValue("@comentario", string.IsNullOrEmpty(encuesta.Comentario) ? "" : encuesta.Comentario);

                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        mensaje = "Encuesta Registrada Correctamente";
                    }
                    else
                    {
                        mensaje = "Error: No se pudo registrar";
                    }
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
