using RevitFormulasValidator.Enums;
using System.Collections.Generic;

namespace RevitFormulasValidator.ViewModels
{
    public class RevitFunctionViewModel
    {
        public RevitEnums.FunctionType FunctionType { get; set; }

        public List<string> Arguments { get; set; }

        public List<RevitFunctionViewModel> ChildFunctions { get; set; }

        public RevitFunctionViewModel()
        {
        }
    }
}
