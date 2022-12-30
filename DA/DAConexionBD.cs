using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DA
{
    public class DAConexionBD
    {
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conn = new SqlConnection("server=10.31.1.220; Initial Catalog=SDMARKETING_ADEX_PRUEBAS; uid=sa; pwd=exporta1; encrypt=False");
            return conn;
        }
    }
}
