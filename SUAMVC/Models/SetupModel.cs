using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{

    public class SetupModel
    {
        private suaEntities db = new suaEntities();

        public SetupModel() { }

        /**
         * Creamos la configuración inicial para empezar a trabajar con 
         * el sistema.
         * */
        public void setUpSystem()
        {
            int count = db.Usuarios.Count();
            if (count == 0)
            {
                Plaza plaza = crearPlazas();
                Role role = crearRoles();
                usuarioRoot(plaza, role);
                crearModulos();
            }//existen usuarios dados de alta?
        }


        /**
         * Creamos los roles
         * */
        private Role crearRoles()
        {

            int count = db.Roles.Count();
            Role role = new Role();

            if (count == 0)
            {
                DateTime date = DateTime.Now;

                role.descripcion = "Ejecutivo";
                role.fechaCreacion = date;
                role.estatus = "A";

                db.Roles.Add(role);

                role = new Role();
                role.descripcion = "Cliente";
                role.fechaCreacion = date;
                role.estatus = "A";

                db.Roles.Add(role);

                role = new Role();
                role.descripcion = "Administrador";
                role.fechaCreacion = date;
                role.estatus = "A";

                db.Roles.Add(role);

                db.SaveChanges();
            }
            else
            {
                role = db.Roles.Find(1);
            }

            return role;
        }
        /**
         * creamos los modulos
         * */
        private void crearModulos()
        {
            int count = db.Modulos.Count();

            if (count == 0)
            {
                Modulo modulo = new Modulo();

                modulo.descripcionCorta = "Seguridad";
                modulo.descripcionLarga = "Administración del Sistema SIAP";
                DateTime date = DateTime.Now;
                modulo.estatus = "A";
                modulo.fechaCreacion = date;

                modulo = db.Modulos.Add(modulo);
                db.SaveChanges();

                //Grabamos las funciones de seguridad
                funcionesSeguridad(modulo.id);
                RoleModulo rm = new RoleModulo();
                rm.moduloId = modulo.id;

                //Buscamos el rol de administrador
                Role role = new Role();
                role = (from x in db.Roles
                        where x.descripcion.Equals("Administrador")
                        select x).FirstOrDefault();

                rm.roleId = role.id;
                rm.usuarioCreacionId = 1;
                rm.fechaCreacion = date;

                db.RoleModulos.Add(rm);
                db.SaveChanges();

                //Damos permisos a funciones de seguridad
                permisosFuncionesSeguridad(role.id, modulo.id);

                modulo = new Modulo();

                modulo.descripcionCorta = "Catalogos";
                modulo.descripcionLarga = "Catalogos del Sistema SIAP";
                modulo.fechaCreacion = date;
                modulo.estatus = "A";

                db.Modulos.Add(modulo);
                db.SaveChanges();

                modulo = new Modulo();

                modulo.descripcionCorta = "IMSS";
                modulo.descripcionLarga = "Manejo del IMSS en SIAP";
                modulo.fechaCreacion = date;
                modulo.estatus = "A";

                db.Modulos.Add(modulo);

                modulo = new Modulo();

                modulo.descripcionCorta = "Carga";
                modulo.descripcionLarga = "Carga de Archivos SUA en SIAP";
                modulo.fechaCreacion = date;
                modulo.estatus = "A";

                db.Modulos.Add(modulo);
                db.SaveChanges();

            }
        }
        /**
         * Creamos las plazas
         * */
        private Plaza crearPlazas()
        {
            int count = db.Plazas.Count();
            Plaza plaza = new Plaza();
            if (count == 0)
            {
                plaza.descripcion = "Local";

                db.Plazas.Add(plaza);
                db.SaveChanges();
            }
            else
            {
                plaza = db.Plazas.Find(1);
            }
            return plaza;
        }
        /**
         * Damos de alta al usuario principal
         * */
        private void usuarioRoot(Plaza plaza, Role role)
        {

            var usuarioRoot = from b in db.Usuarios
                              where b.claveUsuario.Equals("root")
                              select b;

            if (usuarioRoot == null || usuarioRoot.Count() == 0)
            {
                DateTime date = DateTime.Now;
                Usuario usuario = new Usuario();
                usuario.claveUsuario = "root";
                usuario.apellidoPaterno = "SIAP";
                usuario.apellidoMaterno = "Admon";
                usuario.contrasena = "123";
                usuario.email = "user@siap.com.mx";
                usuario.estatus = "A";
                usuario.nombreUsuario = "El Administrador";
                usuario.fechaIngreso = date;
                usuario.Role = role;
                usuario.roleId = role.id;
                usuario.plazaId = plaza.id;
                usuario.Plaza = plaza;

                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }
        }

        private void funcionesSeguridad(int moduloId)
        {
            DateTime date = DateTime.Now;

            //Catalogo de Munciones
            Funcion funcion = new Funcion();
            funcion.descripcionCorta = "Funciones";
            funcion.descripcionLarga = "Catalogo de funciones del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "Funciones";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);

            //Catalogo de Modulos
            funcion = new Funcion();
            funcion.descripcionCorta = "Modulos";
            funcion.descripcionLarga = "Catalogo de modulos del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "Modulos";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);

            //Catalogo de Usuarios
            funcion = new Funcion();
            funcion.descripcionCorta = "Usuarios";
            funcion.descripcionLarga = "Catalogo de usuarios del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "Usuarios";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);

            //Catalogo de Roles
            funcion = new Funcion();
            funcion.descripcionCorta = "Roles";
            funcion.descripcionLarga = "Catalogo de roles del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "Roles";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);

            //Modulos por role
            funcion = new Funcion();
            funcion.descripcionCorta = "Modulos por Rol";
            funcion.descripcionLarga = "Modulos por Rol del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "RoleModulos";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);

            //Funciones por role
            funcion = new Funcion();
            funcion.descripcionCorta = "Funciones por Rol";
            funcion.descripcionLarga = "Funciones por Rol del SIAP";
            funcion.moduloId = moduloId;
            funcion.usuarioId = 1;
            funcion.controlador = "RoleFunciones";
            funcion.accion = "Index";
            funcion.fechaCreacion = date;
            funcion.estatus = "A";

            db.Funcions.Add(funcion);
            db.SaveChanges();
        }

        //Otorgamos permisos a las funciones
        private void permisosFuncionesSeguridad(int roleId, int moduloId)
        {

            var funciones = from x in db.Funcions
                            where x.moduloId.Equals(moduloId)
                            select x;

            if (funciones.Count() > 0)
            {
                DateTime date = DateTime.Now;
                foreach (Funcion fun in funciones)
                {
                    RoleFuncion rf = new RoleFuncion();
                    rf.funcionId = fun.id;
                    rf.roleId = roleId;
                    rf.Funcion = fun;
                    rf.usuarioCreacionId = 1;
                    rf.fechaCreacion = date;

                    db.RoleFuncions.Add(rf);
                    
                }
                db.SaveChanges();
            }



        }
    }


}