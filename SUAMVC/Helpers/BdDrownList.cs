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

            foreach (Plaza item in listPlazas)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas") || descripcion.Contains("Seleccion"))
                {
                    itemId = "";
                }
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

            foreach (Plaza item in listPlazas)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas") || descripcion.Contains("Seleccion"))
                {
                    itemId = "";
                }
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


            foreach (Cliente item in list)
            {
                String itemId = item.Id.ToString().Trim();
                String descripcion = item.claveCliente.Trim() + "-" + item.descripcion.Trim();

                if (item.claveCliente.Trim().Contains("Todos"))
                {
                    itemId = "";
                    descripcion = item.descripcion.Trim();
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("clientesId", listFields, new { onchange = "submit()" });
        }

        public static MvcHtmlString clientesNSDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Cliente> list = (from s in db.Clientes.ToList()
                                  join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                  where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(userId)
                                  orderby s.claveCliente, s.descripcion
                                  select s).ToList();


            foreach (Cliente item in list)
            {
                String itemId = item.Id.ToString().Trim();
                String descripcion = item.claveCliente.Trim() + "-" + item.descripcion.Trim();

                if (item.claveCliente.Trim().Contains("Todos"))
                {
                    itemId = "";
                    descripcion = item.descripcion.Trim();
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("clienteId", listFields);
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

        public static MvcHtmlString esquemasDrownList(this HtmlHelper htmlHelper, int userId, String htmlId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<EsquemasPago> listEsquemas = (from s in db.EsquemasPagoes
                                               orderby s.descripcion
                                               select s).ToList();

            foreach (EsquemasPago item in listEsquemas)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("esquemaId", listFields, new { id = htmlId });
        }

        public static MvcHtmlString contratosDrownList(this HtmlHelper htmlHelper, int userId, String htmlId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<TipoContrato> listContratos = (from s in db.TipoContratoes
                                                orderby s.descripcion
                                                select s).ToList();

            foreach (TipoContrato item in listContratos)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("contratoId", listFields, new { id = htmlId });
        }

        public static MvcHtmlString sexosDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Sexo> listSexos = (from s in db.Sexos
                                    orderby s.descripcion
                                    select s).ToList();

            foreach (Sexo item in listSexos)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("sexoId", listFields);
        }

        public static MvcHtmlString paisesDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Pais> listPaises = (from s in db.Paises
                                     orderby s.naturalez ascending
                                     select s).ToList();

            foreach (Pais item in listPaises)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("paisId", listFields);
        }

        public static MvcHtmlString paisesDrownList(this HtmlHelper htmlHelper, int userId, String componenteId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Pais> listPaises = (from s in db.Paises
                                     orderby s.naturalez ascending
                                     select s).ToList();

            foreach (Pais item in listPaises)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList(componenteId, listFields, new { id = "paisId" });
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

        public static MvcHtmlString estadosDrownList(this HtmlHelper htmlHelper)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Estado> listEstados = (from s in db.Estados
                                        orderby s.descripcion
                                        select s).ToList();

            foreach (Estado item in listEstados)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("estadoId", listFields);
        }

        public static MvcHtmlString tipoPersonalDrownList(this HtmlHelper htmlHelper, int userId, String htmlId, String componenteId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<TipoPersonal> listTipoPersonal = (from s in db.TipoPersonals
                                                   orderby s.descripcion
                                                   select s).ToList();

            foreach (TipoPersonal item in listTipoPersonal)
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
        public static MvcHtmlString conceptosDrownList(this HtmlHelper htmlHelper, int userId, string grupo, string componentId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Concepto> listConceptos = (from s in db.Conceptos
                                            where s.grupo.Equals(grupo.Trim())
                                            orderby s.descripcion
                                            select s).ToList();

            foreach (Concepto item in listConceptos)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Todas"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList(componentId, listFields);

        }
        public static MvcHtmlString proyectoDrownList(this HtmlHelper htmlHelper, int userId, string componenteId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<Proyecto> listProyecto = (from p in db.Proyectos
                                      orderby p.descripcion
                                      select p).ToList();

            foreach (Proyecto item in listProyecto)
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

        public static MvcHtmlString cuentasPorEmpleadoDrownList(this HtmlHelper htmlHelper, int userId, String componenteId)
        {

            db = new suaEntities();
            List<SelectListItem> listFields = new List<SelectListItem>();

            List<CuentaEmpleado> listCuentas = (from s in db.CuentaEmpleadoes.ToList()
                                      where s.usuarioId.Equals(userId) 
                                      orderby s.Banco.descripcion
                                      select s).ToList();

            foreach (CuentaEmpleado item in listCuentas)
            {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.Banco.descripcion.Trim() + " - " + item.cuenta;
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList(componenteId, listFields);
        }
    }
}