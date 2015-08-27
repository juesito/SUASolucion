using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class CrystalReports
    {
        private String reportName {get; set;}
        private String path = @"C:\\SUA\\Layouts\\";

        public CrystalReports() { }

        public CrystalReports(String reportName) {
            this.reportName = reportName;
        }

        //public ReportDocument launchReport() {
        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(path + this.reportName.Trim());
            

            
        //    return rd;
        //}
    }
}