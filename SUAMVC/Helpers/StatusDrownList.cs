using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace SUAMVC.Helpers
{
    public static class StatusDrownList
    {
        public static MvcHtmlString SearchStatusList(this HtmlHelper htmlHelper)
        {
            return htmlHelper.DropDownList("statusId", new List<SelectListItem>() { new SelectListItem { Text = "Todos", Value = "" },  new SelectListItem { Text = "Activo", Value = "A" }, new SelectListItem { Text = "Inactivo", Value = "B" } }, new { onchange = "form.submit();" });
        }

        public static MvcHtmlString FieldsAvailablesToFilter(this HtmlHelper htmlHelper)
        {

            List<SelectListItem> listFields = new List<SelectListItem> {
                              new SelectListItem {Value = "", Text = "Seleccione"},
                              new SelectListItem {Value = "1", Text = "Reg. Patronal"},
                              new SelectListItem {Value = "2", Text = "Num. Afiliación"},
                              new SelectListItem {Value = "3", Text = "CURP"},
                              new SelectListItem {Value = "4", Text = "RFC"},
                              new SelectListItem {Value = "5", Text = "Nombre"},
                              new SelectListItem {Value = "6", Text = "Fecha Alta"},
                              new SelectListItem {Value = "7", Text = "Fecha Baja"},
                              new SelectListItem {Value = "8", Text = "Salario IMMS"},
                              new SelectListItem {Value = "9", Text = "Ubicación"},
                              new SelectListItem {Value = "10", Text = "ID Grupo"},
                              new SelectListItem {Value = "11", Text = "Ocupación"},
                              new SelectListItem {Value = "12", Text = "Plaza"},
                              new SelectListItem {Value = "13", Text = "Extranjero?"}
            };
             
            return htmlHelper.DropDownList("opcion", listFields);
        }

        public static MvcHtmlString FieldsAvailablesToFilterAcreditados(this HtmlHelper htmlHelper)
        {

            List<SelectListItem> listFields = new List<SelectListItem> {
                              new SelectListItem {Value = "", Text = "Seleccione"},
                              new SelectListItem {Value = "1", Text = "Reg. Patronal"},
                              new SelectListItem {Value = "2", Text = "Num. Afiliación"},
                              new SelectListItem {Value = "3", Text = "CURP"},
                              new SelectListItem {Value = "4", Text = "RFC"},
                              new SelectListItem {Value = "5", Text = "Nombre"},
                              new SelectListItem {Value = "6", Text = "Fecha Alta"},
                              new SelectListItem {Value = "7", Text = "Fecha Baja"},
                              new SelectListItem {Value = "8", Text = "Salario IMMS"},
                              new SelectListItem {Value = "9", Text = "Ubicación"},
                              new SelectListItem {Value = "10", Text = "ID Grupo"},
                              new SelectListItem {Value = "11", Text = "Ocupación"},
                              new SelectListItem {Value = "12", Text = "Plaza"}
            };
             
            return htmlHelper.DropDownList("opcion", listFields);
        }

        public static MvcHtmlString FieldsAvailablesToFilterIncapacidades(this HtmlHelper htmlHelper)
        {

            List<SelectListItem> listFields = new List<SelectListItem> {
                              new SelectListItem {Value = "", Text = "Seleccione"},
                              new SelectListItem {Value = "1", Text = "Reg. Patronal"},
                              new SelectListItem {Value = "2", Text = "Num. Afiliación"},
                              new SelectListItem {Value = "3", Text = "CURP"},
                              new SelectListItem {Value = "4", Text = "RFC"},
                              new SelectListItem {Value = "5", Text = "Nombre"},
                              new SelectListItem {Value = "6", Text = "Fecha Alta"},
                              new SelectListItem {Value = "9", Text = "Ubicación"},
                              new SelectListItem {Value = "11", Text = "Ocupación"},
                              new SelectListItem {Value = "12", Text = "Plaza"}
            };

            return htmlHelper.DropDownList("opcion", listFields);
        }

        public static MvcHtmlString topicosList(this HtmlHelper htmlHelper)
        {

            return htmlHelper.DropDownList("topico", new List<SelectListItem>() { 
                new SelectListItem { Text = "Seleccione", Value = "" }, 
                new SelectListItem { Text = "Cliente", Value = "C" }, 
                new SelectListItem { Text = "Grupos", Value = "G" }, 
                new SelectListItem { Text = "Patrones", Value = "B" },
                new SelectListItem { Text = "Plaza", Value = "P" }}, new { onchange = "form.submit();" });
        }
    }
}