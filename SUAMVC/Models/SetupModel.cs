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
        public void setUpSystem(){
            int count = db.Usuarios.Count();
            if (count == 0)
            {
                Plaza plaza = crearPlazas();
                crearModulos();
                Role role = crearRoles();
                usuarioRoot(plaza, role);
            }//existen usuarios dados de alta?
        }


        /**
         * Creamos los roles
         * */
        private Role crearRoles() {

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
            else {
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

            if (count == 0) {
                Modulo modulo = new Modulo();

                modulo.descripcionCorta = "SEGURIDAD";
                modulo.descripcionLarga = "Administración del Sistema SIAP";
                DateTime date = DateTime.Now;
                modulo.estatus = "A";
                modulo.fechaCreacion = date;

                db.Modulos.Add(modulo);
                db.SaveChanges();

                modulo = new Modulo();

                modulo.descripcionCorta = "CATALOGOS";
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

                modulo.descripcionCorta = "CARGA";
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
        private Plaza crearPlazas() {
            int count = db.Plazas.Count();
            Plaza plaza = new Plaza();
            if (count == 0)
            {
                plaza.descripcion = "Local";

                db.Plazas.Add(plaza);
                db.SaveChanges();
            }
            else {
                plaza = db.Plazas.Find(1);
            }
            return plaza;
        }
        /**
         * Damos de alta al usuario principal
         * */
        private void usuarioRoot(Plaza plaza, Role role) {

            var usuarioRoot = from b in db.Usuarios
                              where b.claveUsuario.Equals("root")
                              select b;

            if (usuarioRoot == null || usuarioRoot.Count() == 0) {
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
    }

    
}