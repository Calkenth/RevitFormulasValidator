using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RevitFormulasValidator.Enums
{
    public class RevitEnums
    {
        public enum FunctionType
        {
            SimpleFunction = 0,
            [Description("if")]
            IfFunction,
            [Description("or")]
            OrFunction,
            [Description("and")]
            AndFunction,
            [Description("not")]
            NotFunction,
            [Description("round")]
            RoundFunction,
            [Description("roundup")]
            RoundUpFunction,
            [Description("rounddown")]
            RoundDownFunction
        }

        public enum FunctionStage
        {
            Open,
            Close
        }
    }
}
