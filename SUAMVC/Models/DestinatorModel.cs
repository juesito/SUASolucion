using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class DestinatorModel
    {
        public DestinatorModel() { }
        public DestinatorModel( String email)
        {
            this.name = "Auxiliar";
            this.email = email;
        }
        public DestinatorModel(String name, String email) {
            this.name = name;
            this.email = email;
        }
        public String name { get; set; }
        public String email { get; set; }
    }
}