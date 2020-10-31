using RevitFormulasValidator.Data;
using RevitFormulasValidator.Enums;
using RevitFormulasValidator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitFormulasValidator
{
    public class FormulaGenerator
    {
        public string Generate(List<RevitFunctionViewModel> revitFunctions)
        {
            if (revitFunctions.Count == 1)
            {
                if (revitFunctions.First().FunctionType == RevitEnums.FunctionType.SimpleFunction)
                {
                    string simpleFunction = String.Empty;

                    foreach (var arg in revitFunctions.First().Arguments)
                    {
                        simpleFunction += arg;
                    }

                    return simpleFunction;
                }
            }

            string formulaBody = String.Empty;

            foreach (var function in revitFunctions)
            {
                formulaBody += GenerateBody(function);
            }

            return formulaBody;
        }

        private string GenerateBody(RevitFunctionViewModel revitFunction)
        {
            var formulaBody = FunctionTypeAnalizer.GetBlueprint(revitFunction.FunctionType);
            for (int i = 0; i < revitFunction.Arguments.Count; i++)
            {
                string arg = (string)revitFunction.Arguments[i];
                if (arg == "Function")
                {
                    var childFunction = revitFunction.ChildFunctions.First();
                    arg = GenerateBody(childFunction);
                    revitFunction.ChildFunctions.Remove(childFunction);
                }

                if ((revitFunction.FunctionType == RevitEnums.FunctionType.OrFunction ||
                    revitFunction.FunctionType == RevitEnums.FunctionType.AndFunction) &&
                    i > 1)
                {
                    formulaBody = formulaBody.Replace(Convert.ToString(formulaBody.Last()), $",{{{i}}})");
                }
                formulaBody = formulaBody.Replace($"{{{i}}}", arg);
            }

            return formulaBody;
        }
    }
}
