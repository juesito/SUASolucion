﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using SUADATOS;
using SUAMVC.Models;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;


using System.Data.Entity.Validation;
using System.Text;
using SUAMVC.Helpers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.IO;

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

                    string[] substrings = Regex.Split(emailListParameter.valorString.Trim(), ";");

                    foreach (String indice in substrings)
                    {
                        DestinatorModel destinator = new DestinatorModel("CIAH", indice);
                        email.to.Add(destinator);
                    }
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
            if (!String.IsNullOrEmpty(cliente.emailContacto)) {

                string[] substrings = Regex.Split(cliente.emailContacto.Trim(), ";");

                foreach (String indice in substrings)
                {
                    DestinatorModel destinator = new DestinatorModel("CIAH", indice);
                    email.to.Add(destinator);
                }
            }

            // Mail del usuaria logueado
            if (!String.IsNullOrEmpty(solicitud.Usuario.email))
            {
                DestinatorModel destinator = new DestinatorModel("CIAH", solicitud.Usuario.email.Trim());
                email.to.Add(destinator);
            }

            // Mail del ejecutivo del cliente
            if (cliente.Usuario != null)
            {

                if (!String.IsNullOrEmpty(cliente.Usuario.email))
                {
                    DestinatorModel destinator = new DestinatorModel("CIAH", cliente.Usuario.email.Trim());
                    email.to.Add(destinator);
                }
            }
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
                email.msg = "Se ha generado una nueva solicitud de Baja de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajador(es)";
            }//Es solicitud de Baja de personal?
            else if (tipo.Equals("M")) {
                email.subject = "Folio modificación:" + solicitud.folioSolicitud;
                email.msg = "Se ha generado una nueva solicitud de Modificación de Personal con Folio : " + solicitud.folioSolicitud + "del cliente " +
                    cliente.descripcion.ToUpper() + ", el cual contiene " + solicitud.noTrabajadores +
                    " trabajador(es)";
            }

            //Enviamos el email ya preparado.
            this.EnviarEmail(email, solicitud);

        }


        public bool EnviarEmail(EmailModel email, Solicitud solicitud)
        {
            //Preparamos el mensaje del email
            MailMessage msg = new MailMessage();
            Boolean isSended = false;
            email.from = "info@desarrolloytalento.com";

            //vaciamos las direcciones de correo
            foreach (DestinatorModel dm in email.to) {
                msg.To.Add(dm.email.Trim());
            }

            String msgHeader = "Solicitud del cliente: " + solicitud.Cliente.descripcion.Trim();

            msg.From = new MailAddress(email.from, msgHeader , System.Text.Encoding.UTF8);
            msg.Subject = email.subject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = email.msg;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false; //Si vas a enviar un correo con contenido html entonces cambia el valor a true

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("info@desarrolloytalento.com", "/info2016*");
            client.Port = 587;
//            client.Port = 3535;
//            client.Host = "smtpout.secureserver.net";//Este es el smtp valido para Gmail
            client.Host = "smtp.office365.com";//Este es el smtp valido para Gmail
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