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
            //SqlConnection conn = new SqlConnection("server=10.31.1.220; Initial Catalog=SDGAF_ADEX_PRUEBAS; uid=sa; pwd=exporta1; encrypt=False");
            SqlConnection conn = new SqlConnection("server=10.31.1.18; Initial Catalog=SDMARKETING_ADEX; uid=usrprdmrc; pwd=8eQ1ApVPfH");
            return conn;
        }
    }
}
