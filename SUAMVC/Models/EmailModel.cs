using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class EmailModel
    {
        public EmailModel() {
            to = new List<DestinatorModel>();
        }
        public String msg{get; set;}
        public List<DestinatorModel> to { get; set; }
        public String from { get; set; }
        public String body { get; set; }
        public String subject { get; set; }

    }
}