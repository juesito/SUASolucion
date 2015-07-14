using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SUADATOS;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SUAMVC.Helpers
{
    public static class BdDrownList
    {
        static suaEntities db = new suaEntities();

        /*
         * DrownList para las plazas
         */
        public static MvcHtmlString plazasDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Plaza> listPlazas = (from s in db.Plazas.ToList()
                                      join top in db.TopicosUsuarios on s.id equals top.topicoId
                                      where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(userId)
                                      && !s.descripcion.Contains("Local")
                                      orderby s.cveCorta, s.descripcion
                                      select s).ToList();

            String itemId = "";
            String descr = "Todas";
            listFields.Add(new SelectListItem { Value = itemId, Text = descr });
            foreach (Plaza item in listPlazas)
            {
                itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                //if (descripcion.Contains("Todas") || descripcion.Contains("Seleccion"))
                //{
                //    itemId = "";
                //}
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("plazasId", listFields, new { onchange = "submit()" });
        }

        public static MvcHtmlString plazasDrownListNS(this HtmlHelper htmlHelper, int userId, string idHtml)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Plaza> listPlazas = (from s in db.Plazas.ToList()
                                      join top in db.TopicosUsuarios on s.id equals top.topicoId
                                      where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(userId)
                                      && !s.descripcion.Contains("Local")
                                      orderby s.cveCorta, s.descripcion
                                      select s).ToList();

            String itemId = "";
            String descr ="Todas";
            listFields.Add(new SelectListItem { Value = itemId, Text = descr });
            foreach (Plaza item in listPlazas)
            {
                itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
//                if (descripcion.Contains("Todas") || descripcion.Contains("Seleccion"))
//                {
//                    itemId = "";
//                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("plazaId", listFields, new { id = idHtml });
        }
        /*
         * DrownList para las patrones
         */
        public static MvcHtmlString patronesDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();            
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Patrone> list = (from s in db.Patrones.ToList()
                                  join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                  where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(userId)
                                  orderby s.registro
                                  select s).ToList();


            String itemId = "";
            String descr = "Todos";
            listFields.Add(new SelectListItem { Value = itemId, Text = descr });
            foreach (Patrone item in list)
            {
                itemId = item.Id.ToString().Trim();
                String descripcion = item.registro.Trim() + "-" + item.nombre.Trim();

                //if (descripcion.Contains("Todos") || descripcion.Contains("Seleccion") )
                //{
                //    itemId = "";
                //    descripcion = item.nombre.Trim();
                //}
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("patronesId", listFields, new { onchange = "submit()" });
        }

        public static MvcHtmlString patronesNoChangeDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Patrone> list = (from s in db.Patrones.ToList()
                                  join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                  where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(userId)
                                  orderby s.registro
                                  select s).ToList();


            foreach (Patrone item in list)
            {
                String itemId = item.Id.ToString().Trim();
                String descripcion = item.registro.Trim() + "-" + item.nombre.Trim();

                if (descripcion.Contains("Todos") || descripcion.Contains("Seleccion"))
                {
                    itemId = "";
                    descripcion = item.nombre.Trim();
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("patronesId", listFields);
        }

        public static MvcHtmlString patronesNoChangeKeysDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Patrone> list = (from s in db.Patrones.ToList()
                                  join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                  where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(userId)
                                  orderby s.registro
                                  select s).ToList();


            foreach (Patrone item in list)
            {
                String itemId = item.Id.ToString().Trim();
                String descripcion = item.registro.Trim() + "-" + item.nombre.Trim();

                if (descripcion.Contains("Todos") || descripcion.Contains("Seleccion"))
                {
                    itemId = "";
                    descripcion = item.nombre.Trim();
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("patronesId", listFields);
        }

        public static MvcHtmlString clientesDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Cliente> list = (from s in db.Clientes.ToList()
                                  join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                  where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(userId)
                                  orderby s.claveCliente, s.descripcion
                                  select s).ToList();


            String itemId = "";
            String descr = "Todos";
            listFields.Add(new SelectListItem { Value = itemId, Text = descr });
            foreach (Cliente item in list)
            {
                itemId = item.Id.ToString().Trim();
                String descripcion = item.claveCliente.Trim() + "-" + item.descripcion.Trim();

                //if (item.claveCliente.Trim().Contains("Todos"))
                //{
                //    itemId = "";
                //    descripcion = item.descripcion.Trim();
                //}
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("clientesId", listFields, new { onchange = "submit()" });
        }

        public static MvcHtmlString gruposDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Grupos> list = (from s in db.Grupos.ToList()
                                 join cli in db.Clientes on s.Id equals cli.Grupo_id
                                 join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                 where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(userId)
                                 orderby s.claveGrupo, s.nombre
                                 select s).Distinct().ToList();

            foreach (Grupos item in list)
            {
                String itemId = item.Id.ToString().Trim();
                String descripcion = item.claveGrupo.Trim() + "-" + item.nombre.Trim();

                if (item.claveGrupo.Trim().Contains("Todos"))
                {
                    itemId = "";
                    descripcion = item.nombre.Trim();
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("gruposId", listFields, new { onchange = "submit()" });
        }


        public static MvcHtmlString modulosDrownList(this HtmlHelper htmlHelper)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Modulo> listModulos = (from s in db.Modulos
                                     orderby s.descripcionCorta 
                                     select s).ToList();

            foreach (Modulo item in listModulos)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcionCorta.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("moduloId", listFields);
        }


        public static MvcHtmlString bancosDrownList(this HtmlHelper htmlHelper, int userId, String componenteId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Banco> listBancos = (from s in db.Bancos
                                                   orderby s.descripcion
                                                   select s).ToList();

            foreach (Banco item in listBancos)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList(componenteId, listFields);
        }

        /**
         * ActionImage para incluir una imagen en un link
         * 
         */
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string imagePath, string alt)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            if (!String.IsNullOrEmpty(imagePath))
            {
                imgBuilder.MergeAttribute("src", url.Content(imagePath));
            }
            else
            {
                imgBuilder.MergeAttribute("src", url.Content("~/Content/Images/camera.png"));
            }
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        //DrowList para Motivo de Baja
     }
}