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
        public static MvcHtmlString plazasDrownList(this HtmlHelper htmlHelper, int userId) {

            db = new suaEntities();
            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(userId)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            List<SelectListItem> listFields = new List<SelectListItem>();
            
            List<Plaza> listPlazas = (from s in db.Plazas.ToList()
                            join top in db.TopicosUsuarios on s.id equals top.topicoId
                                          where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(userId)
                            orderby s.cvecorta, s.descripcion select s).ToList();

            foreach (Plaza item in listPlazas) {
                String itemId = item.id.ToString().Trim();
                String descripcion = item.descripcion.Trim();
                if (descripcion.Contains("Seleccion")) {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("patronesId", listFields, new { onchange = "submit()" });
        }
        /*
         * DrownList para las patrones
         */
        public static MvcHtmlString patronesDrownList(this HtmlHelper htmlHelper, int userId)
        {

            db = new suaEntities();
            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(userId)
                                     && x.tipo.Equals("B")
                                     select x.topicoId); 

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
                if (descripcion.Contains("Seleccion"))
                {
                    itemId = "";
                }
                listFields.Add(new SelectListItem { Value = itemId, Text = descripcion.Trim() });
            }

            return htmlHelper.DropDownList("plazasId", listFields, new { onchange = "submit()" });
        }
    }
}