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
    public class DAEstado
    {
        public List<BEEstado> GetListaEstados()
        {
            List<BEEstado> listaEstados = new List<BEEstado>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_ESTADOS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BEEstado reg = new BEEstado
                    {
                        IdEstado = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        EsActivo = dr.GetBoolean(2)
                    };

                    listaEstados.Add(reg);
                }

                dr.Close(); con.Close();
            }

            return listaEstados;
        }

        public List<BEEstado> GetListaEstadosActivos()
        {
            return GetListaEstados().Where(e => e.EsActivo).ToList();
        }
    }
}
