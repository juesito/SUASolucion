using SUADATOS;
using SUAMVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public static class SecurityUserModel
    {
        private static suaEntities db;
        private static List<RoleFuncion> roleFunciones;

        //Recogemos los permisos del perfil
        public static void llenarPermisos(int roleId)
        {
            db = new suaEntities();
            roleFunciones = db.RoleFuncions.Where(x => x.roleId.Equals(roleId)
                && x.Funcion.tipo.Trim().Equals("A")).ToList();
        }

        //Verficamos si se tiene permiso al modulo función
        public static Boolean verificarPermiso(String modulo, String funcion, int moduloId)
        {
            Boolean perfilConPermiso = false;
            ParametrosHelper helper = new ParametrosHelper();

            Parametro parametro = helper.getParameterByKey("ENVIROMENT");

            if (!String.IsNullOrEmpty(parametro.valorString))
            {
                if (!parametro.valorString.Trim().Equals("D"))
                {
                    if (modulo.Equals("1") && funcion.Equals("1"))
                    {
                        perfilConPermiso = true;
                        return perfilConPermiso;
                    }

                    Parametro llenarFunciones = helper.getParameterByKey("FULLFUNC");
                    if (llenarFunciones.valorString.Trim().Equals("T")) {
                        Funcion funcionPorAutorizar = db.Funcions.Where(f => f.descripcionCorta.Trim().Equals(modulo.Trim()) 
                            && f.descripcionLarga.Trim().Equals(funcion.Trim())).FirstOrDefault();

                        if (funcionPorAutorizar == null) {
                            funcionPorAutorizar = new Funcion();
                            funcionPorAutorizar.descripcionCorta = modulo.Trim();
                            funcionPorAutorizar.descripcionLarga = funcion.Trim();
                            funcionPorAutorizar.tipo = "A";
                            funcionPorAutorizar.fechaCreacion = DateTime.Now;
                            funcionPorAutorizar.estatus = "A";
                            funcionPorAutorizar.usuarioId = 1;
                            funcionPorAutorizar.moduloId = moduloId;
                            funcionPorAutorizar.accion = "N/A";
                            funcionPorAutorizar.controlador = "N/A";

                            db.Funcions.Add(funcionPorAutorizar);
                            db.SaveChanges();
                        }
                    }

                    if (roleFunciones != null && roleFunciones.Count() > 0)
                    {
                        RoleFuncion roleFuncion = roleFunciones
                            .Where(x => x.Funcion.descripcionCorta.Trim().Equals(modulo)
                             && x.Funcion.descripcionLarga.Trim().Equals(funcion)).FirstOrDefault();

                        if (roleFuncion != null)
                        {
                            perfilConPermiso = true;
                        }
                    }
                }
                else
                {
                    perfilConPermiso = true;
                } // Ambiente de desarrollo
            }

            return perfilConPermiso;

        }

        public static void limpiarListaDePermisos(){
            if (roleFunciones != null && roleFunciones.Count() > 0)
            {
                roleFunciones.Clear();
            }
        }
    }
}