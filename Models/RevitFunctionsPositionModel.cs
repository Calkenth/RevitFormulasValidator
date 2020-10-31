using RevitFormulasValidator.Enums;

namespace RevitFormulasValidator.Models
{
    public class RevitFunctionsPositionModel
    {
        public int Position { get; set; }

        public RevitEnums.FunctionStage Stage { get; set; }
    }
}
