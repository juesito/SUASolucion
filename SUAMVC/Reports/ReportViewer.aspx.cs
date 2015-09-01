using CrystalDecisions.CrystalReports.Engine;
using SUAMVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SUAMVC.Reports
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CrystalReports cr = new CrystalReports("test.rpt");
                //ReportDocument rp = cr.launchReport();

                //rp.SetDatabaseLogon("root", "jeargaqu");
                //rp.VerifyDatabase();
                //CrystalReportViewer1.ReportSource = rp;
                //CrystalReportViewer1.DisplayToolbar = true;
                //CrystalReportViewer1.RefreshReport();
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }
    }
}