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
    public class DATicket
    {
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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

        public string AsignarTicket(int idTicket, int idUsuarioResponsable, int idCategoria, int idSubCategoria, int IdEstado)
        {
            string mensaje = "";
            var usuarioResponsable = gestionUsuario.BuscarUsuario(idUsuarioResponsable);

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Asignar_Ticket", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);

                    if (idUsuarioResponsable > 0)
                        cmd.Parameters.AddWithValue("@id_usuario_responsable", idUsuarioResponsable);
                    else
                        cmd.Parameters.AddWithValue("@id_usuario_responsable", DBNull.Value);

                    if (idCategoria > 0)
                        cmd.Parameters.AddWithValue("@id_categoria", idCategoria);
                    else
                        cmd.Parameters.AddWithValue("@id_categoria", DBNull.Value);

                    if (idSubCategoria > 0)
                        cmd.Parameters.AddWithValue("@id_subcategoria", idSubCategoria);
                    else
                        cmd.Parameters.AddWithValue("@id_subcategoria", DBNull.Value); 
                    
                    if (IdEstado > 0)
                        cmd.Parameters.AddWithValue("@id_estado", IdEstado);
                    else
                        cmd.Parameters.AddWithValue("@id_estado", DBNull.Value);

                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                    con.Open();
                    int rs = cmd.ExecuteNonQuery();

                    if (rs >= 1)
                    {

                        var detalleTicket = DetallarTicket(idTicket);
                        mensaje = NotificarUsuarioAsignacion(usuarioResponsable, detalleTicket);
                        mensaje = "Ticket Asignado correctamente";

                        if (IdEstado == 3)
                        {
                            string msj = EnviarEncuesta(detalleTicket);
                        }
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

        public DetalleTicketViewModel DetallarTicket(int idTicket)
        {
            DetalleTicketViewModel detalleTicket = new DetalleTicketViewModel();

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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
                    detalleTicket.CategoriaTicket = dr.GetString(5);
                    detalleTicket.SubCategoriaTicket = dr.GetString(6);
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

                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    string dominio = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");
                    string enlace = dominio + "Correo/AbrirCorreos/" + detalleTicket.IdTicket;

                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.gaf@adexperu.edu.pe", "@dex2023");
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
                                "Mesa de Servicio GAF - ADEX",

                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Ticket Asignado - " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.gaf@adexperu.edu.pe")
                    };

                    message.To.Add(usuario.CorreoUsuario);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

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

        public string ActualizarEstadoTicket(int idTicket, int idEstado, int IdCategoria, int IdSubcategoria)
        {
            string mensaje = "";
            DetalleTicketViewModel detalleTicket = DetallarTicket(idTicket);
            SqlConnection con = DAConexionBD.ObtenerConexion();
            con.Open();
            SqlTransaction tr = con.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("usp_Actualizar_Estado_Ticket", con, tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                cmd.Parameters.AddWithValue("@id_estado", idEstado);
                cmd.Parameters.AddWithValue("@id_categoria", IdCategoria);
                cmd.Parameters.AddWithValue("@id_subcategoria", IdSubcategoria);


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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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
                        //IdSubcategoria = (dr.IsDBNull(2)) ? 0 : dr.GetInt32(2),
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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

                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("servicedesk.gaf@adexperu.edu.pe", "@dex2023");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Encuesta de satisfacción ticket: " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.gaf@adexperu.edu.pe")
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

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
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
        public string UsuarioResponsableDeAsignación(BE.BEUsuario usuario, int idTicket)
        {
            string mensaje = "";
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Asignar_AsignadoPor", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                    cmd.Parameters.AddWithValue("@id", usuario.IdUsuario);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    mensaje = "Asignado por, realizado correctamente";
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

        public string GenerarFlujo(int idTicket, int idUsuarioResponsable, string tema, int[] tipoDeContenido, string descripcion, DateTime fechaEntrega)
        {
            string mensaje = "";
            int pri = 0;
            int numPiezas = GenerarTipoDeContenidoTicket(idTicket, tipoDeContenido);
            var usuarioResponsable = gestionUsuario.BuscarUsuario(idUsuarioResponsable);
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_Asignar_Flujo", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                    //
                    cmd.Parameters.AddWithValue("@id_usuario_modificacion", usuario.IdUsuario);
                    if (idUsuarioResponsable > 0)
                        cmd.Parameters.AddWithValue("@id_usuario_resposable", idUsuarioResponsable);
                    else
                        cmd.Parameters.AddWithValue("@id_usuario_resposable", DBNull.Value);
                    //
                    cmd.Parameters.AddWithValue("@tema", tema);
                    cmd.Parameters.AddWithValue("@numeroPiezas", numPiezas);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    DateTime hoy = DateTime.Now;
                    TimeSpan dias = (TimeSpan)(fechaEntrega - hoy);
                    int valPrioridad = dias.Days;
                    if (valPrioridad == 1 || valPrioridad == 0 || valPrioridad < 0)
                    {
                        pri = 1;
                    }
                    else
                    {
                        if (valPrioridad == 2 || valPrioridad == 3)
                        {
                            pri = 2;
                        }
                        else
                        {
                            pri = 3;
                        }
                    }
                    cmd.Parameters.AddWithValue("@prioridad", pri);
                    cmd.Parameters.AddWithValue("@fechaEntrega", fechaEntrega);


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
                string mensaje1 = NotificarUsuarioAsignacion(usuarioResponsable, detalleTicket);
                string mensaje2 = NotificarClienteAsignacion(detalleTicket);
                mensaje = "Asignado Correctamente";
            }

            return mensaje;
        }

        public string NotificarClienteAsignacion(BE.ViewModels.DetalleTicketViewModel detalleTicket)
        {
            string mensaje = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    string dominio = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");

                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.gaf@adexperu.edu.pe", "@dex2023");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        Body = "Estimado(a) " + ": </br>" +
                                "Le brindamos la actualización del estado de su ticket Generado a Service Desk GAF:" + "</br>" +
                                "<b>Asunto: </b>" + detalleTicket.AsuntoTicket + "</br>" +
                                "<b>Fecha de Generación: </b>" + detalleTicket.FechaGeneracion + "</br>" +
                                "<b>Fecha de Entrega: </b>" + Convert.ToDateTime(detalleTicket.fechaEntrega).ToString("dd/MM/yyyy") + "</br>" +
                                "<b>Etapa: </b>" + detalleTicket.EstadoTicket + "</br>" +
                                "<b>Estado: </b>" + detalleTicket.etapa + "</br> </br>" +
                                "<b>¡Favor de NO responder este correo!</b> </br> </br>" +
                                "Atemtamente, </br>" +
                                "Mesa de Servicio ADEX GAF",

                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Actualización de Ticket - " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.gaf@adexperu.edu.pe")
                    };

                    message.To.Add(detalleTicket.PropietarioTicket);

                    client.Send(message);
                    client.Dispose();

                    mensaje = "Asignación y Notificación enviadas correctamente al cliente";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al enviar correo: " + ex.Message;
            }

            return mensaje;
        }

        public int GenerarTipoDeContenidoTicket(int id, int[] tipoDeContenido)
        {
            int valor = 0;
            foreach (var item in tipoDeContenido)
            {
                valor = TDCTicketRepetido(id, item);
                if (valor == 0)
                {
                    using (SqlConnection con = DAConexionBD.ObtenerConexion())
                    {

                        try
                        {
                            SqlCommand cmd = new SqlCommand("usp_asignar_TDC_Ticket", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_ticket", id);
                            cmd.Parameters.AddWithValue("@id_tipoDeContenido", item);

                            con.Open();
                            cmd.ExecuteNonQuery();

                            //respuesta = true;
                        }
                        catch (Exception ex)
                        {
                            //respuesta = false;
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            int numPiezas = NumeroDePiezas(id);
            return numPiezas;
        }

        public int TDCTicketRepetido(int idTicket, int tipoDeContenido)
        {

            int valor = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Listar_TDC_Ticket", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                cmd.Parameters.AddWithValue("@id_tipoDeContenido", tipoDeContenido);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr.IsDBNull(1))
                        {
                            valor = 0;
                        }
                        else
                        {
                            valor = dr.GetInt32(1);
                        }
                    }
                }
                else
                {
                    valor = 0;
                }
                dr.Close();
                con.Close();
            }
            return valor;
        }

        public int NumeroDePiezas(int idTicket)
        {

            int valor = 0;
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("usp_Obtener_numeroDePiezas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_ticket", idTicket);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr.IsDBNull(0))
                        {
                            valor = 0;
                        }
                        else
                        {
                            valor = dr.GetInt32(0);
                        }
                    }
                }
                else
                {
                    valor = 0;
                }
                dr.Close();
                con.Close();
            }
            return valor;
        }
        /*
        public string EliminarPedido(int id, string mensaje)
        {
            string respuesta = "";

            var detalleTicket = DetallarTicket(id);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //string dominio = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");
                    //string enlace = dominio + "Correo/AbrirCorreos/";

                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.gaf@adexperu.edu.pe", "@dex2023");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        Body = "Estimado(a) " + ": </br> </br>" +
                                "<b>Su solicitud: </b> </br>" +
                                "<b>Asunto: </b>" + detalleTicket.AsuntoTicket + "</br>" +
                                "<b>Fecha Generación: </b>" + detalleTicket.FechaGeneracion + "</br>" +
                                "Ha sido desestimada por: </br> </br>" +
                                mensaje + "</br>" + "</br>" +
                                //"<b>Asunto: </b>" + detalleTicket.AsuntoTicket + "</br>" +
                                //"<b>Fecha Generación: </b>" + detalleTicket.FechaGeneracion + "</br>" +
                                //"<b>Fecha Cierre: </b>" + detalleTicket.GetFechaFormateada(detalleTicket.FechaCierre) + "</br>" +
                                //"<b>Categoría: </b>" + detalleTicket.CategoriaTicket + "</br>" +
                                //"<b>Subcategoría: </b>" + detalleTicket.SubCategoriaTicket + "</br> </br>" +
                                //"Puede revisar el ticket en el siguiente enlace: <a href='" + enlace + "' >Visualizar Ticket</a> </br> </br> " +
                                "<b>¡Favor de NO responder este correo!</b> </br> </br>" +
                                "Atemtamente, </br>" +
                                "Mesa de Servicio ADEX RRHH",

                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Solicitud Rechazada - " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.gaf@adexperu.edu.pe")
                    };

                    message.To.Add(usuario.CorreoUsuario);

                    client.Send(message);
                    client.Dispose();

                    ActualizarEstadoTicket(id, 6, 0, 0);

                    mensaje = "Notificación eliminada correctamente";
                }
            }
            catch (Exception ex)
            {
                respuesta = "Error al enviar correo: " + ex.Message;
            }

            return respuesta;
        }
        */

        public string EnviarActualizacionDeEstadoTicket(int idTicket)
        {
            string mensaje = "";
            BE.ViewModels.DetalleTicketViewModel detalleTicket = DetallarTicket(idTicket);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (SmtpClient client = new SmtpClient("smtp.office365.com", 587))
                {
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    string dominio = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + urlHelper.Content("~");
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential("servicedesk.gaf@adexperu.edu.pe", "@dex2023");
                    MailMessage message = new MailMessage()
                    {
                        IsBodyHtml = true,
                        Body = "Estimado(a) " + ": </br>" +
                                "Se ha actualizado su ticket: </br> </br>" +
                                 "<b>Asunto: </b>" + detalleTicket.AsuntoTicket + "</br>" +
                                "<b>Fecha de Generación: </b>" + detalleTicket.FechaGeneracion + "</br>" +
                                "<b>Fecha de Entrega: </b>" + Convert.ToDateTime(detalleTicket.fechaEntrega).ToString("dd/MM/yyyy") + "</br>" +
                                "<b>Etapa: </b>" + detalleTicket.EstadoTicket + "</br>" +
                                "<b>Estado: </b>" + detalleTicket.etapa + "</br> </br>" +
                                "Atemtamente, </br>" +
                                "Mesa de Servicio ADEX GAF",

                        BodyEncoding = System.Text.Encoding.UTF8,
                        Subject = "Ticket Actualizado - " + detalleTicket.AsuntoTicket,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        From = new MailAddress("servicedesk.gaf@adexperu.edu.pe")
                    };

                    message.To.Add(detalleTicket.PropietarioTicket);

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

        public string ActualizarFechaTicket(int idTicket, DateTime fechaEntrega)
        {
            string mensaje = "";

            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_Actualizar_Fecha_Ticket", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                    cmd.Parameters.AddWithValue("@fechaEntrega", fechaEntrega);

                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        mensaje = "Fecha actualizada correctamente";
                    }
                    else
                    {
                        mensaje = "Error: No se pudo actualizar la fecha";
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

        public string ActualizarPrioridad(int idTicket, DateTime fechaEntrega)
        {
            int pri = 0;
            string mensaje = "";
            //
            DateTime hoy = DateTime.Now;
            TimeSpan dias = (TimeSpan)(fechaEntrega - hoy);
            int valPrioridad = dias.Days;
            if (valPrioridad == 1 || valPrioridad == 0 || valPrioridad < 0)
            {
                pri = 1;
            }
            else
            {
                if (valPrioridad == 2 || valPrioridad == 3)
                {
                    pri = 2;
                }
                else
                {
                    pri = 3;
                }
            }
            //
            using (SqlConnection con = DAConexionBD.ObtenerConexion())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_Actualizar_Prioridad", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_ticket", idTicket);
                    cmd.Parameters.AddWithValue("@prioridad", pri);

                    if (cmd.ExecuteNonQuery() >= 1)
                    {
                        mensaje = "Prioridad actualizada correctamente";
                    }
                    else
                    {
                        mensaje = "Error: No se pudo actualizar la fecha";
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
    }
}
