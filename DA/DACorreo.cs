using BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AE.Net.Mail;
using System.Web;
using DA.Enums;

namespace DA
{
    internal class DACorreo
    {
        private readonly DAUtilidades gestionUtilidades = new DAUtilidades();

        public List<BECorreo> BuscarCorreos(int idCorreo)
        {
            List<BECorreo> listaCorreos = new List<BECorreo>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("USP_BUSCAR_CORREO", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCorreo", idCorreo);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    BECorreo reg = new BECorreo()
                    {
                        IdCorreo = dr.GetInt32(0),
                        //Remitente = dr.GetString(1),
                        AsuntoCorreo = dr.GetString(2),
                        FechaEnvio = dr.GetDateTime(3),
                        MensajeCorreo = dr.GetString(4),
                        CodigoCorreo = dr.GetString(5),
                        //Copias = dr.GetString(6),
                        //IdRespuesta = dr.GetString(7),
                    };
                    listaCorreos.Add(reg);
                }

                dr.Close(); con.Close();
            }

            return listaCorreos;
        }

        public string Responder(BECorreo correo, string copias, string para, HttpPostedFileBase[] adjuntos)
        {
            string mensaje = "";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.ti@adexperu.org.pe", "@dex2021");
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage()
                    {
                        IsBodyHtml = true,
                        Body = correo.MensajeCorreo,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = correo.AsuntoCorreo,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.ti@adexperu.org.pe")
                    };

                    message.To.Add(para);

                    message.CC.Add("servicedesk.ti@adexperu.org.pe");

                    if (!string.IsNullOrEmpty(copias))
                    {
                        message.CC.Add(copias);
                    }

                    AlternateView alterView = ContentToAlternateView(correo.MensajeCorreo, message);
                    message.AlternateViews.Add(alterView);

                    if (adjuntos != null && !adjuntos.Contains(null))
                    {
                        foreach (var adjunto in adjuntos)
                        {
                            message.Attachments.Add(new System.Net.Mail.Attachment(adjunto.InputStream, adjunto.FileName));
                        }
                    }

                    message.Headers.Add("In-Reply-To", correo.CodigoCorreo);
                    message.Headers.Add("References", correo.CodigoCorreo + " " + correo.HeaderReferences);

                    client.Send(message);
                    //RegistrarRespuesta(correo.CodigoCorreo);

                    mensaje = "Respuesta Enviada - Recuerda Actualizar el Estado del Ticket";
                }
            }
            catch (Exception e)
            {
                mensaje = "Error: " + e.Message;
            }

            return mensaje;
        }

        private static AlternateView ContentToAlternateView(string content, System.Net.Mail.MailMessage message)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };

                message.Attachments.Add(new System.Net.Mail.Attachment(Base64ToImageStream(base64), "imagen_" + imgCount + ".jpg"));
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }

        public string RegistrarRespuesta(string codigoCorreRespondido)
        {

            String respuesta = "";

            SqlConnection con = DAConexionBD.ObtenerConexion();
            con.Open();
            SqlTransaction tr = con.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // Encuentra la ultima respuesta registrada para el correo
                ImapClient ic = new ImapClient("imap-mail.outlook.com", "servicedesk.ti@adexperu.org.pe", "@dex2021", AuthMethods.Login, 993, true);
                ic.SelectMailbox("inbox");
                // Obtiene la cantidad de respuestas a ese correo
                var cantidad = ic.SearchMessages(SearchCondition.Header("In-Reply-To", codigoCorreRespondido), false, false).Count();
                // Obtiene el último correo para registrarlo
                var correo = ic.SearchMessages(SearchCondition.Header("In-Reply-To", codigoCorreRespondido), false, false)
                                                .ToList()[cantidad - 1].Value;

                string rutaAdjuntos = HttpContext.Current.Server.MapPath(@"~\Adjuntos\");

                // Variables del correo
                string codigoCorreo = correo.MessageID;
                DateTime fechaEnvio = correo.Date;
                string asunto = correo.Subject;
                string mensaje = correo.Body;
                string enRespuestaA = correo.Headers["In-Reply-to"].Value;
                var datosRemitente = correo.From;

                string nombreCarpetaAdjuntos = codigoCorreo.Substring(1, codigoCorreo.Length - 2);

                int idRemitente = ObtenerIdDireccionCorreo(datosRemitente.Address, con, tr);

                if (idRemitente == 0)
                {
                    idRemitente = RegistrarDireccionCorreo(datosRemitente.Address, datosRemitente.DisplayName, con, tr);
                }

                foreach (var direccion in correo.To)
                {
                    int idDireccionCorreo = ObtenerIdDireccionCorreo(direccion.Address, con, tr);
                    Console.WriteLine("\t -" + direccion);
                    if (idDireccionCorreo == 0) // Sí no existe una dirección registrada
                    {
                        idDireccionCorreo = RegistrarDireccionCorreo(direccion.Address, direccion.DisplayName, con, tr);
                    }

                    RegistrarCorreoDireccionCorreo(codigoCorreo, idDireccionCorreo, 1, con, tr);
                }

                Console.WriteLine("CC: ");
                foreach (var direccion in correo.Cc)
                {
                    int idDireccionCorreo = ObtenerIdDireccionCorreo(direccion.Address, con, tr);
                    Console.WriteLine("\t -" + direccion);
                    if (idDireccionCorreo == 0) // Sí no existe una dirección registrada
                    {
                        idDireccionCorreo = RegistrarDireccionCorreo(direccion.Address, direccion.DisplayName, con, tr);
                    }

                    RegistrarCorreoDireccionCorreo(codigoCorreo, idDireccionCorreo, 2, con, tr);
                }

                Directory.CreateDirectory(rutaAdjuntos + codigoCorreo.Substring(1, codigoCorreo.Length - 2));
                for (int i = 0; i < correo.Attachments.Count; i++)
                {
                    var adjunto = correo.Attachments.ElementAt(i);
                    string cidAdjunto = adjunto.Headers["Content-ID"].Value;

                    if (adjunto.ContentType.Contains("image"))
                    {
                        adjunto.Save(rutaAdjuntos + nombreCarpetaAdjuntos + "\\imagen_" + i + ".jpg");
                        RegistrarAdjunto(codigoCorreo, nombreCarpetaAdjuntos + "\\imagen_" + i + ".jpg", "imagen_" + i, cidAdjunto, con, tr);
                    }
                    else
                    {
                        adjunto.Save(rutaAdjuntos + nombreCarpetaAdjuntos + "\\" + adjunto.Filename);
                        RegistrarAdjunto(codigoCorreo, nombreCarpetaAdjuntos + "\\" + adjunto.Filename, adjunto.Filename, cidAdjunto, con, tr);
                    }

                }

                BECorreo correoRegistro = new BECorreo()
                {
                    CodigoCorreo = codigoCorreo,
                    FechaEnvio = fechaEnvio,
                    AsuntoCorreo = asunto,
                    MensajeCorreo = mensaje,
                    EnRespuestaA = enRespuestaA,
                    IdRemitente = 0,
                    IdTicket = 0,
                };

                int idTicket = ObtenerIdTicketRespuesta(enRespuestaA, con, tr);

                correoRegistro.IdTicket = idTicket;
                correoRegistro.IdRemitente = idRemitente;

                RegistrarCorreo(correoRegistro, con, tr);

                // Valida que no se haya registrado mediante el servicio

                if (!ExisteCorreo(codigoCorreo))
                {
                    tr.Commit();
                }

                ic.Disconnect();
                ic.Dispose();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                respuesta = ex.Message;
            }
            finally
            {
                con.Close();
            }

            return respuesta;
        }

        static void RegistrarCorreo(BECorreo correo, SqlConnection con, SqlTransaction tr)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("usp_Registrar_Correo", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", correo.CodigoCorreo);
                cmd.Parameters.AddWithValue("@fecha_envio", correo.FechaEnvio);
                cmd.Parameters.AddWithValue("@asunto_correo", correo.AsuntoCorreo);
                cmd.Parameters.AddWithValue("@mensaje_correo", correo.MensajeCorreo);
                cmd.Parameters.AddWithValue("@id_remitente", correo.IdRemitente);
                cmd.Parameters.AddWithValue("@en_resupuesta_a", correo.EnRespuestaA);
                cmd.Parameters.AddWithValue("@id_ticket", correo.IdTicket);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método RegistrarCorreo() " + ex.Message);
            }

        }

        static int ObtenerIdDireccionCorreo(string direccionCorreo, SqlConnection con, SqlTransaction tr)
        {
            int idDireccionCorreo = 0;

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Obtener_Id_Direccion_Correo", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@direccion_correo", direccionCorreo);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    idDireccionCorreo = dr.GetInt32(0);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método ObtenerIdDireccionCorreo() " + ex.Message);
            }

            return idDireccionCorreo;
        }

        static int ObtenerIdTicketRespuesta(string enRepuestaA, SqlConnection con, SqlTransaction tr)
        {
            int idTicket = 0;

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Obtener_Id_Ticket_Respuesta", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@en_respuesta_a", enRepuestaA);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    idTicket = dr.GetInt32(0);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método ObtenerIdTicketRespuesta() " + ex.Message);
            }

            return idTicket;
        }

        static int RegistrarDireccionCorreo(string direccionCorreo, string usuarioCorreo, SqlConnection con, SqlTransaction tr)
        {
            int idDireccionCorreo = 0;

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Registrar_Direccion_Correo", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@direccion_correo", direccionCorreo);
                cmd.Parameters.AddWithValue("@usuario_correo", usuarioCorreo);

                SqlParameter salida = new SqlParameter()
                {
                    ParameterName = "@id_direccion_correo",
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(salida);

                cmd.ExecuteNonQuery();

                idDireccionCorreo = int.Parse(salida.Value.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método RegistrarDireccionCorreo() " + ex.Message);
            }

            return idDireccionCorreo;
        }

        static void RegistrarCorreoDireccionCorreo(string codigoCorreo, int idDireccionCorreo, int idTipoRelacion, SqlConnection con, SqlTransaction tr)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Registrar_Correo_Direccion_Correo", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", codigoCorreo);
                cmd.Parameters.AddWithValue("@id_direccion_correo", idDireccionCorreo);
                cmd.Parameters.AddWithValue("@id_tipo_relacion", idTipoRelacion);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método RegistrarCorreoDireccionCorreo() " + ex.Message);
            }
        }

        static void RegistrarAdjunto(string codigoCorreo, string rutaAdjunto, string nombreAdjunto, string cidAdjunto, SqlConnection con, SqlTransaction tr)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("usp_Registrar_Adjunto", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", codigoCorreo);
                cmd.Parameters.AddWithValue("@ruta_adjunto", "~/Adjuntos/" + rutaAdjunto);
                cmd.Parameters.AddWithValue("@nombre_adjunto", nombreAdjunto);
                cmd.Parameters.AddWithValue("@cid_adjunto", cidAdjunto);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Error en el método RegistrarAdjunto() " + ex.Message);
            }
        }

        public bool ExisteCorreo(string codigoCorreo)
        {
            bool existe;

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Validar_Existencia_Correo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codigo_correo", codigoCorreo);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                existe = dr.Read();
                dr.Close();
                con.Close();
            }

            return existe;
        }

        // Métodos verificados

        public IEnumerable<BECorreo> ListarCorreosPorTicket(int idTicket)
        {
            List<BECorreo> correos = new List<BECorreo>();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Correos_Por_Ticket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    BECorreo correo = new BECorreo()
                    {
                        IdCorreo = dr.GetInt32(0),
                        CodigoCorreo = dr.GetString(1),
                        FechaEnvio = dr.GetDateTime(2),
                        AsuntoCorreo = dr.GetString(3),
                        MensajeCorreo = dr.GetString(4),
                        IdRemitente = dr.GetInt32(5),
                        Remitente = dr.GetString(6),
                        IdTicket = dr.GetInt32(7),
                        CorreoRemitente = dr.GetString(8),
                        HeaderReferences = dr.IsDBNull(9) ? "" : dr.GetString(9)
                    };

                    correo.Destinatarios = gestionUtilidades.ListarDireccionesCorreo(correo.CodigoCorreo, (int)TipoRelacionCorreo.Para);
                    correo.Copias = gestionUtilidades.ListarDireccionesCorreo(correo.CodigoCorreo, (int)TipoRelacionCorreo.Copia);

                    correos.Add(correo);
                }

                dr.Close();
                con.Close();
            }
            return correos;
        }

        public BECorreo BuscarCorreo(string codigo_correo, int idTicket)
        {
            var correos = ListarCorreosPorTicket(idTicket);
            BECorreo correo = correos.Where(c => c.CodigoCorreo == codigo_correo).FirstOrDefault();
            if (correo != null)
            {
                correo.Destinatarios = gestionUtilidades.ListarDireccionesCorreo(correo.CodigoCorreo, (int)TipoRelacionCorreo.Para);
                correo.Copias = gestionUtilidades.ListarDireccionesCorreo(correo.CodigoCorreo, (int)TipoRelacionCorreo.Copia);
            }

            return correo;
        }

    }
}
