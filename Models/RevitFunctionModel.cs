using Newtonsoft.Json;
using RevitFormulasValidator.Enums;
using System.Collections.Generic;

namespace RevitFormulasValidator.Models
{
    public class RevitFunctionModel
    {
        public RevitEnums.FunctionType FunctionType { get; private set; }
        public string FunctionTypeName => FunctionType.ToString();
        public List<string> Arguments { get; private set; }
        public List<RevitFunctionModel> ChildFunctions { get; private set; }

        [JsonIgnore]
        public int StartPos { get; set; }
        [JsonIgnore]
        public RevitFunctionModel ParentFunction { get; set; }

        public RevitFunctionModel(RevitEnums.FunctionType functionType)
        {
            FunctionType = functionType;
            ChildFunctions = new List<RevitFunctionModel>();
            Arguments = new List<string>();
        }

        public void AddArgument(string argument)
        {
            Arguments.Add(argument);
        }

        public void AddChildFunction(RevitFunctionModel function)
        {
            function.ParentFunction = this;
            ChildFunctions.Add(function);
        }
    }
}
