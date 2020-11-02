using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevitFormulasValidator.ViewModels
{
    public class Response
    {
        public bool Success { get; set; }

        public string Content { get; set; }

        public string Error { get; set; }
    }
}
