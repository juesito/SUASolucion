using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using SUADATOS;
using SUAMVC.Models;

namespace SUAMVC.Helpers
{
    public class Email
    {
        suaEntities db = new suaEntities();

        EmailModel email = new EmailModel();

        public void enviarPorClienteTipo(int clienteId, String tipo, int solicitudId){
            
            Solicitud solicitud = (from sol in db.Solicituds
                                       where sol.id.Equals(solicitudId)
                                       select sol).First();
            
            Cliente cliente = (from d in db.Clientes
                              where d.Id.Equals(clienteId)
                              select d).First();

            ListaValidacionCliente datos = (from s in db.ListaValidacionClientes
                                            where s.clienteId.Equals(clienteId)
                                                select s).First();

            if (tipo.Equals("S")) {
                email.subject = "Folio Solicitud:" + solicitud.folioSolicitud;
                email.msg = "Se ha generado una nueva solicitud de Alta de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + "con Esquema " + solicitud.EsquemasPago.descripcion + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajadores";
            }else if (tipo.Equals("B")) {
                email.subject = "Folio baja:" + solicitud.folioSolicitud;
                email.msg = "Se ha generado una nueva solicitud de Alta de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + "con Esquema " + solicitud.EsquemasPago.descripcion + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajador(es)";
            }

            this.EnviarEmail(email);

        }

        public bool EnviarEmail()
        {

            MailMessage msg = new MailMessage();
            Boolean isSended = false;
            msg.To.Add("jues40@hotmail.com");
            msg.From = new MailAddress("jueser@gmail.com", "Jesus armando", System.Text.Encoding.UTF8);
            msg.Subject = "Ke onda vato";
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = "Best Regards";
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false; //Si vas a enviar un correo con contenido html entonces cambia el valor a true
            //Aquí es donde se hace lo especial

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("jueser@gmail.com", "Spike&&Nadh");
            client.Port = 587;
            client.Host = "smtp.gmail.com";//Este es el smtp valido para Gmail
            client.EnableSsl = true; //Esto es para que vaya a través de SSL que es obligatorio con GMail

            try
            {
                client.Send(msg);

                isSended = false;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo electrónico: " + ex.Message);

            }
            return isSended;
        }

        public bool EnviarEmail(EmailModel email)
        {

            MailMessage msg = new MailMessage();
            Boolean isSended = false;
            msg.To.Add(email.to);
            msg.From = new MailAddress(email.from, "Jesus armando", System.Text.Encoding.UTF8);
            msg.Subject = email.subject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = email.msg;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false; //Si vas a enviar un correo con contenido html entonces cambia el valor a true
            //Aquí es donde se hace lo especial

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("jueser@gmail.com", "Spike&&Nadh");
            client.Port = 587;
            client.Host = "smtp.gmail.com";//Este es el smtp valido para Gmail
            client.EnableSsl = true; //Esto es para que vaya a través de SSL que es obligatorio con GMail

            try
            {
                client.Send(msg);

                isSended = false;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo electrónico: " + ex.Message);

            }
            return isSended;
        }
    }
}