using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DA
{
    public class DAAdjunto
    {
        public List<BEAdjunto> GetAdjuntosCorreo(int idCorreo)
        {
            List<BEAdjunto> listaAdjuntos = new List<BEAdjunto>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_ADJUNTOS_CORREO", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCorreo", idCorreo);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEAdjunto reg = new BEAdjunto();
                    reg.IdAdjunto = dr.GetInt32(0);
                    reg.RutaAdjunto = dr.GetString(1);
                    reg.IdCorreo = dr.GetInt32(2);
                    reg.NombreAdjunto = dr.GetString(3);

                    listaAdjuntos.Add(reg);
                }

                dr.Close(); con.Close();
            }

            return listaAdjuntos;
        }

        public List<BEAdjunto> GetAdjuntosCorreoRespuesta(int idCorreo)
        {
            List<BEAdjunto> listaAdjuntos = new List<BEAdjunto>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_ADJUNTOS_RESPUESTA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCorreo", idCorreo);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEAdjunto reg = new BEAdjunto();
                    reg.IdAdjunto = dr.GetInt32(0);
                    reg.RutaAdjunto = dr.GetString(1);
                    reg.IdCorreo = dr.GetInt32(2);

                    listaAdjuntos.Add(reg);
                }

                dr.Close(); con.Close();
            }

            return listaAdjuntos;
        }
    }
}
