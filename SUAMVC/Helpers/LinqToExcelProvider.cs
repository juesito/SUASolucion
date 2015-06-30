using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class LinqToExcelProvider
    {
        /// <summary>
        /// Obtenemos el nombre de archivo excel
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Template connectionstring para Excel connections
        /// esta cadena es para archivos excel 2007 y 2010
        /// </summary>
        private const string ConnectionStringTemplateOld = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;HDR=YES;IMEX=1;";
        //
        private const string ConnectionStringTemplate = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0 Xml;";
        //para archivos de 97-2003 usar la siguiente cadena
        private string ConnectionStringTemplateVeryOld = "provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fileName">The Excel file to process</param>
        public LinqToExcelProvider(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Returns a worksheet as a linq-queryable enumeration
        /// </summary>
        /// <param name="sheetName">The name of the worksheet</param>
        /// <returns>An enumerable collection of the worksheet</returns>
        public EnumerableRowCollection<DataRow> GetWorkSheet(string sheetName)
        {
            // Build the connectionstring
            string connectionString = string.Format(ConnectionStringTemplate, FileName);

            // Query the specified worksheet
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), connectionString);

            // Fill the dataset from the data adapter
            DataSet myDataSet = new DataSet();
            dataAdapter.Fill(myDataSet, "ExcelInfo");
            //dataAdapter.Fill(myDataSet);

            // Initialize a data table which we can use to enumerate the contents based on the dataset
            DataTable dataTable = myDataSet.Tables["ExcelInfo"];

            // Return the data table contents as a queryable enumeration
            return dataTable.AsEnumerable();
        }
    }
}