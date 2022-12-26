using BE.ViewModels;
using BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DA
{
    internal class DATicket
    {
        private readonly string _cadena;
        private readonly DAUsuario gestionUsuario;
        private readonly BEUsuario usuario;

        public DATicket()
        {
            gestionUsuario = new DAUsuario();
            usuario = HttpContext.Current.Session["usuario"] != null ? HttpContext.Current.Session["usuario"] as BEUsuario : new BEUsuario();
        }

        // Métodos verificados
        public IEnumerable<TicketViewModel> ListarTickets()
        {
            List<TicketViewModel> tickets = new List<TicketViewModel>();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Tickets", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketViewModel ticket = new TicketViewModel()
                    {
                        IdTicket = dr.GetInt32(0),
                        FechaGeneracion = dr.GetDateTime(1),
                        AsuntoTicket = dr.GetString(2),
                        EstadoTicket = dr.GetString(3),
                        InicialesResponsable = dr.GetString(4),
                        UsuarioResponsable = dr.GetString(5)
                    };

                    if (dr.IsDBNull(6))
                    {
                        ticket.FechaAsignacion = null;
                    }
                    else
                    {
                        ticket.FechaAsignacion = dr.GetDateTime(6);
                    }

                    if (dr.IsDBNull(7))
                    {
                        ticket.FechaCierre = null;
                    }
                    else
                    {
                        ticket.FechaCierre = dr.GetDateTime(7);
                    }

                    tickets.Add(ticket);
                }

                dr.Close();
                con.Close();
            }

            return tickets;
        }

        public IEnumerable<TicketViewModel> FiltrarTickets(string condicionEstado, string condicionResponsable)
        {
            List<TicketViewModel> tickets = new List<TicketViewModel>();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Filtrar_Tickets", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@condicion_estado", condicionEstado);
                cmd.Parameters.AddWithValue("@condicion_responsable", condicionResponsable);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketViewModel ticket = new TicketViewModel()
                    {
                        IdTicket = dr.GetInt32(0),
                        FechaGeneracion = dr.GetDateTime(1),
                        AsuntoTicket = dr.GetString(2),
                        EstadoTicket = dr.GetString(3),
                        InicialesResponsable = dr.GetString(4),
                        UsuarioResponsable = dr.GetString(5),
                    };

                    tickets.Add(ticket);
                }

                dr.Close();
                con.Close();
            }

            return tickets;
        }

        public IEnumerable<TicketViewModel> FiltrarMisTickets(string condicionEstado, int idResponsable)
        {
            List<TicketViewModel> tickets = new List<TicketViewModel>();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Filtrar_Mis_Tickets", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@condicion_estado", condicionEstado);
                cmd.Parameters.AddWithValue("@id_responsable", idResponsable);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketViewModel ticket = new TicketViewModel()
                    {
                        IdTicket = dr.GetInt32(0),
                        FechaGeneracion = dr.GetDateTime(1),
                        FechaAsignacion = dr.GetDateTime(2),
                        AsuntoTicket = dr.GetString(4),
                        EstadoTicket = dr.GetString(5),
                    };

                    if (dr.IsDBNull(3))
                    {
                        ticket.FechaCierre = null;
                    }
                    else
                    {
                        ticket.FechaCierre = dr.GetDateTime(3);
                    }

                    tickets.Add(ticket);
                }

                dr.Close();
                con.Close();
            }

            return tickets;
        }

        public IEnumerable<TicketViewModel> ListarTicketsPorResponsable(int idResponsable)
        {
            List<TicketViewModel> tickets = new List<TicketViewModel>();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_Tickets_Responsable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_responsable", idResponsable);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketViewModel ticket = new TicketViewModel
                    {
                        IdTicket = dr.GetInt32(0),
                        FechaGeneracion = dr.GetDateTime(1),
                        FechaAsignacion = dr.GetDateTime(2),
                        AsuntoTicket = dr.GetString(4),
                        EstadoTicket = dr.GetString(5),
                    };

                    if (dr.IsDBNull(3))
                    {
                        ticket.FechaCierre = null;
                    }
                    else
                    {
                        ticket.FechaCierre = dr.GetDateTime(3);
                    }

                    tickets.Add(ticket);
                }

                dr.Close();
                con.Close();
            }

            return tickets;
        }

        public string AsignarTicket(int idTicket, int idUsuarioResponsable, int idSubCategoria)
        {
            string mensaje = "";
            var usuarioResponsable = gestionUsuario.BuscarUsuario(idUsuarioResponsable);

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Asignar_Ticket", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);
                    if (idUsuarioResponsable > 0)
                        cmd.Parameters.AddWithValue("@id_usuario_resposable", idUsuarioResponsable);
                    else
                        cmd.Parameters.AddWithValue("@id_usuario_resposable", DBNull.Value);
                    if (idSubCategoria > 0)
                        cmd.Parameters.AddWithValue("@id_categoria", idSubCategoria);
                    else
                        cmd.Parameters.AddWithValue("@id_categoria", DBNull.Value);

                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Ticket Asignado correctamente";
                }
                catch (Exception ex)
                {
                    mensaje = "Error: " + ex.Message;
                }
                finally
                {
                    con.Close();
                }

                var detalleTicket = DetallarTicket(idTicket);
                mensaje = NotificarUsuarioAsignacion(usuarioResponsable, detalleTicket);
            }

            return mensaje;
        }

        public DetalleTicketViewModel DetallarTicket(int idTicket)
        {
            DetalleTicketViewModel detalleTicket = new DetalleTicketViewModel();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Detallar_Ticket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    detalleTicket.AsuntoTicket = dr.GetString(0);
                    detalleTicket.FechaGeneracion = dr.GetDateTime(1);
                    detalleTicket.UsuarioResponsable = dr.GetString(2);
                    detalleTicket.FechaAsignación = dr.IsDBNull(3) ? (DateTime?)null : dr.GetDateTime(3);
                    detalleTicket.FechaCierre = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
                    detalleTicket.SubCategoriaTicket = dr.GetString(5);
                    detalleTicket.CategoriaTicket = dr.GetString(6);
                    detalleTicket.EstadoTicket = dr.GetString(7);
                    detalleTicket.IdTicket = dr.GetInt32(8);
                    detalleTicket.PropietarioTicket = dr.IsDBNull(9) ? "-" : dr.GetString(9);
                    detalleTicket.GuidTicket = dr.GetGuid(10).ToString();
                }

                dr.Close();
                con.Close();
            }

            return detalleTicket;
        }

        public string NotificarUsuarioAsignacion(BEUsuario usuario, DetalleTicketViewModel detalleTicket)
        {
            string mensaje = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    string dominio = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");
                    string enlace = dominio + "Correo/AbrirCorreos/" + detalleTicket.IdTicket;

                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.ti@adexperu.org.pe", "@dex2021");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        Body = "Estimado(a) " + usuario.NombreCompleto + ": </br>" +
                                "Se le ha asignado un ticket con el siguiente detalle: </br> </br>" +
                                "<b>Asunto: </b>" + detalleTicket.AsuntoTicket + "</br>" +
                                "<b>Fecha Generación: </b>" + detalleTicket.FechaGeneracion + "</br>" +
                                "<b>Fecha Cierre: </b>" + detalleTicket.GetFechaFormateada(detalleTicket.FechaCierre) + "</br>" +
                                "<b>Categoría: </b>" + detalleTicket.CategoriaTicket + "</br>" +
                                "<b>Subcategoría: </b>" + detalleTicket.SubCategoriaTicket + "</br> </br>" +
                                "Puede revisar el ticket en el siguiente enlace: <a href='" + enlace + "' >Visualizar Ticket</a> </br> </br> " +
                                "<b>¡Favor de NO responder este correo!</b> </br> </br>" +
                                "Atemtamente, </br>" +
                                "Mesa de Servicio ADEX",

                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Ticket Asignado - " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.ti@adexperu.org.pe")
                    };

                    message.To.Add(usuario.CorreoUsuario);

                    client.Send(message);
                    client.Dispose();

                    mensaje = "Asignación y Notificación enviadas correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al enviar correo: " + ex.Message;
            }

            return mensaje;
        }

        public string ActualizarEstadoTicket(int idTicket, int idEstado)
        {
            string mensaje = "";
            DetalleTicketViewModel detalleTicket = DetallarTicket(idTicket);
            SqlConnection con = new SqlConnection(_cadena);
            con.Open();
            SqlTransaction tr = con.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Actualizar_Estado_Ticket", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                cmd.Parameters.AddWithValue("@id_estado", idEstado);


                if (cmd.ExecuteNonQuery() >= 1)
                {
                    mensaje = "Estado actualizado correctamente";

                    if (idEstado == 3) // Estado cerrado
                    {
                        mensaje = EnviarEncuesta(detalleTicket);
                    }
                }
                else
                {
                    mensaje = "Error: No se actualizó el estado";
                }

                tr.Commit();
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

            return mensaje;
        }

        public BETicket ListarDatosAsignacion(int idTicket)
        {
            BETicket ticket = null;

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_Datos_Asignacion_Ticket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    ticket = new BETicket()
                    {
                        IdTicket = dr.GetInt32(0),
                        IdUsuarioResponsable = (dr.IsDBNull(1)) ? 0 : dr.GetInt32(1),
                        IdSubcategoria = (dr.IsDBNull(2)) ? 0 : dr.GetInt32(2),
                        IdCategoria = (dr.IsDBNull(3)) ? 0 : dr.GetInt32(3)
                    };
                }

                dr.Close(); con.Close();
            }

            return ticket;
        }

        public string ActualizarPropietario(int idTicket, string propietario)
        {
            string mensaje = "";

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_Actualizar_Propietario_Ticket", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                    cmd.Parameters.AddWithValue("@propietario", propietario);

                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        mensaje = "Propietario actualizado correctamente";
                    }
                    else
                    {
                        mensaje = "Error: No se pudo actualizar el propietario";
                    }

                    con.Close();
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

        public string EnviarEncuesta(DetalleTicketViewModel detalleTicket)
        {
            string mensaje = "";
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string urlBase = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");
            string enlaceEncuesta = $"<a href='{urlBase}Encuestas/Registrar?guidTicket={detalleTicket.GuidTicket}'>Encuesta de satisfacción</a>";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("servicedesk.ti@adexperu.org.pe", "@dex2021");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Encuesta de satisfacción ticket: " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.ti@adexperu.org.pe")
                    };

                    message.Body = $"Por favor responda la encuesta a través del siguiente link: {enlaceEncuesta} <br><br> "
                                   + "¡Vamos por más! <br>"
                                   + "Soluciones TI ADEX";

                    message.To.Add(detalleTicket.PropietarioTicket);

                    client.Send(message);

                    mensaje = "Estado actualizado y encuesta enviada";
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }

            return mensaje;
        }

        public DetalleTicketViewModel GetDetalleByGuid(string guidTicket)
        {
            DetalleTicketViewModel detalleTicket = new DetalleTicketViewModel();

            using (SqlConnection con = new SqlConnection(_cadena))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Buscar_Ticket_GUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@guid_ticket", guidTicket);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    detalleTicket = new DetalleTicketViewModel()
                    {
                        AsuntoTicket = dr.GetString(0),
                        FechaCierre = dr.IsDBNull(1) ? (DateTime?)null : dr.GetDateTime(1),
                        IdTicket = dr.GetInt32(2)
                    };
                }

                con.Close();
            }

            return detalleTicket;
        }

    }
}
