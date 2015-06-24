using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace SUAMVC.Helpers
{
    public class Email
    {
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
    }
}