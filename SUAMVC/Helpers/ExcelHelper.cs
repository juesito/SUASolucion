using LinqToExcel;
using LinqToExcel.Domain;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class ExcelHelper
    {
        public List<PersonalExcelLayout> getPersonalDatos(String rutaDelArchivoExcel) {

            var book = new ExcelQueryFactory(rutaDelArchivoExcel);
            book.DatabaseEngine = DatabaseEngine.Ace;
            book.ReadOnly = true;
            var artistAlbums = from a in book.Worksheet("datos") select a;

            foreach (var a in artistAlbums)
            {
                string artistInfo = "Artist Name: {0}; Album: {1}";
                Console.WriteLine(string.Format(artistInfo, a["Nombre"], a["ApellidoMaterno"]));
            }
            var datos = (from row in book.Worksheet("datos")
                             let item = new PersonalExcelLayout
                             {
                                nombre = row["Nombre"].Cast<String>(),
                                apellidoMaterno = row["ApellidoMaterno"].Cast<String>(),
                                apellidoPaterno = row["ApellidoPaterno"].Cast<String>(),
                                edad = row["Edad"].Cast<int>()
   
                             }
                             select item).ToList();
            book.Dispose();
            return datos;
        }
    }
}