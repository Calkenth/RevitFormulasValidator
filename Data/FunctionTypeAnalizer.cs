using RevitFormulasValidator.Enums;
using RevitFormulasValidator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevitFormulasValidator.Data
{
    public static class FunctionTypeAnalizer
    {
        public static int CheckNumberOfArgs(RevitEnums.FunctionType functionType)
        {
            int numOfArgs;
            switch (functionType)
            {
                case RevitEnums.FunctionType.IfFunction:
                    numOfArgs = 3;
                    break;
                case RevitEnums.FunctionType.NotFunction:
                    numOfArgs = 1;
                    break;
                case RevitEnums.FunctionType.RoundUpFunction:
                    numOfArgs = 1;
                    break;
                case RevitEnums.FunctionType.RoundDownFunction:
                    numOfArgs = 1;
                    break;
                case RevitEnums.FunctionType.RoundFunction:
                    numOfArgs = 1;
                    break;
                case RevitEnums.FunctionType.OrFunction:
                    numOfArgs = 0;
                    break;
                case RevitEnums.FunctionType.AndFunction:
                    numOfArgs = 0;
                    break;
                default:
                    numOfArgs = 0;
                    break;
            }

            return numOfArgs;
        }
        public static string GetBlueprint(RevitEnums.FunctionType functionType)
        {
            string functionBlueprint = String.Empty;
            switch (functionType)
            {
                case RevitEnums.FunctionType.IfFunction:
                    functionBlueprint = "IF({0},{1},{2})";
                    break;
                case RevitEnums.FunctionType.OrFunction:
                    functionBlueprint = "OR({0},{1})";
                    break;
                case RevitEnums.FunctionType.AndFunction:
                    functionBlueprint = "AND({0},{1})";
                    break;
                case RevitEnums.FunctionType.NotFunction:
                    functionBlueprint = "NOT({0})";
                    break;
                case RevitEnums.FunctionType.RoundFunction:
                    functionBlueprint = "ROUND({0})";
                    break;
                case RevitEnums.FunctionType.RoundUpFunction:
                    functionBlueprint = "ROUNDUP({0})";
                    break;
                case RevitEnums.FunctionType.RoundDownFunction:
                    functionBlueprint = "ROUNDDOWN({0})";
                    break;
            }

            return functionBlueprint;
        }
    }
}
