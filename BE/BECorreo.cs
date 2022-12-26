using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BE
{
    public class BECorreo
    {
        public int IdCorreo { get; set; }
        public string CodigoCorreo { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string AsuntoCorreo { get; set; }
        public string MensajeCorreo { get; set; }
        public int IdRemitente { get; set; }
        public string Remitente { get; set; }
        public string EnRespuestaA { get; set; }
        public int IdTicket { get; set; }
        public HtmlDocument Render { get; set; }
        public List<BEDireccionCorreo> Copias { get; set; }
        public List<BEDireccionCorreo> Destinatarios { get; set; }
        public List<BEAdjunto> Adjuntos { get; set; }
        public string CorreoRemitente { get; set; }
        public string HeaderReferences { get; set; }

        public string CopiasTexto
        {
            get
            {
                string listaTexto = "";

                if (Copias != null)
                {
                    for (int i = 0; i < Copias.Count(); i++)
                    {
                        if (Copias[i].Direccion != "service.desk.mktg@adexperu.org.pe")
                        {
                            if (i < Copias.Count() - 1)
                            {
                                listaTexto += (Copias[i].Direccion + ",");
                            }
                            else
                            {
                                listaTexto += (Copias[i].Direccion);
                            }
                        }

                    }
                }

                return listaTexto;
            }

            set { CopiasTexto = value; }

        }

        public string DestinatariosTexto
        {
            get
            {
                string listaTexto = "";

                if (Destinatarios != null)
                {
                    for (int i = 0; i < Destinatarios.Count(); i++)
                    {
                        if (Destinatarios[i].Direccion != "service.desk.mktg@adexperu.org.pe")
                        {
                            if (i < Destinatarios.Count() - 1)
                            {
                                listaTexto += (Destinatarios[i].Direccion + ",");
                            }
                            else
                            {
                                listaTexto += (Destinatarios[i].Direccion);
                            }
                        }
                    }
                }

                return listaTexto;
            }
            set { CopiasTexto = value; }
        }
    }
}
