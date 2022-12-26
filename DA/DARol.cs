using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA
{
    public class DARol
    {
        public List<BE.BERol> GetListaRoles()
        {
            List<BE.BERol> listaRoles = new List<BE.BERol>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_LISTAR_ROLES", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BE.BERol reg = new BE.BERol()
                    {
                        IdRol = dr.GetInt32(0),
                        NombreRol = dr.GetString(1)
                    };

                    listaRoles.Add(reg);
                }

                dr.Close();
                con.Close();
            }

            return listaRoles;
        }
    }
}
