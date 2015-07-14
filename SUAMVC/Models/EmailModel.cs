using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class EmailModel
    {
        public String msg{get; set;}
        public String to { get; set; }
        public String from { get; set; }
        public String body { get; set; }
        public String subject { get; set; }

    }
}