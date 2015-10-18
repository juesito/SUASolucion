﻿using System;
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

        public Email() {
            email = new EmailModel();
        }


        public void enviarPorClienteTipo(String tipo, int solicitudId, bool enviarAListaDistribucion)
        {

            if (enviarAListaDistribucion) {
                ParametrosHelper ph = new ParametrosHelper();
                Parametro emailListParameter = ph.getParameterByKey("EMAILLIST");

                if (!String.IsNullOrEmpty(emailListParameter.valorString)) {
                    DestinatorModel destinator = new DestinatorModel("CIAH" ,emailListParameter.valorString.Trim());
                    email.to.Add(destinator);
                }
            }

            Solicitud solicitud = (from sol in db.Solicituds
                                   where sol.id.Equals(solicitudId)
                                   select sol).First();

            int clienteId = solicitud.clienteId;

            Cliente cliente = (from d in db.Clientes
                               where d.Id.Equals(clienteId)
                               select d).First();

            ////Obtenemos el email del cliente
            //if (!String.IsNullOrEmpty(cliente.email)) {
            //    DestinatorModel destinator = new DestinatorModel(cliente.nombre.Trim(), cliente.email.Trim());
            //    email.to.Add(destinator);
            //}

            ////Emails adicionales del cliente si es que tiene capturados
            //foreach (ListaValidacionCliente lvc in cliente.ListaValidacionClientes) {
            //    if (!String.IsNullOrEmpty(lvc.emailAutorizador)) {
            //        DestinatorModel destinator = new DestinatorModel(lvc.autorizador, lvc.emailAutorizador);
            //        email.to.Add(destinator);
            //    }//Tiene capturado email autorizador?
            //    if (!String.IsNullOrEmpty(lvc.emailValidador))
            //    {
            //        DestinatorModel destinator = new DestinatorModel(lvc.validador, lvc.emailValidador);
            //        email.to.Add(destinator);
            //    }//Tiene capturado email validador?
            //    if (!String.IsNullOrEmpty(lvc.listaEmailAux))
            //    {
            //        DestinatorModel destinator = new DestinatorModel(lvc.listaEmailAux);
            //        email.to.Add(destinator);
            //    }//Tiene capturado email auxiliar?
            //}//Recorremos las listas de validaciones del cliente.

            if (tipo.Equals("A"))
            {
                email.subject = "Folio Solicitud:" + solicitud.folioSolicitud;
                email.msg = "Se ha generado una nueva solicitud de Alta de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + "con Esquema " + solicitud.EsquemasPago.descripcion + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajadores";
            } //Es solicitud de alta de personal?
            else if (tipo.Equals("B"))
            {
                email.subject = "Folio baja:" + solicitud.folioSolicitud;
                email.msg = "Se ha generado una nueva solicitud de Alta de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajador(es)";
            }//Es solicitud de Baja de personal?

            //Enviamos el email ya preparado.
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
            //Preparamos el mensaje del email
            MailMessage msg = new MailMessage();
            Boolean isSended = false;
            email.from = "jueser@gmail.com";

            //vaciamos las direcciones de correo
            foreach (DestinatorModel dm in email.to) {
                msg.To.Add(dm.email.Trim());
            }

            msg.From = new MailAddress(email.from, "Sistma CIAH" , System.Text.Encoding.UTF8);
            msg.Subject = email.subject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = email.msg;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false; //Si vas a enviar un correo con contenido html entonces cambia el valor a true

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