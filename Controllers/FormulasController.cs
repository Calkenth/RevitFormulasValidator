using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RevitFormulasValidator.FormulasWorkers;
using RevitFormulasValidator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitFormulasValidator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
        private readonly FormulaAnalizer formulaAnalizer;
        private readonly FormulaGenerator formulaGenerator;

        public FormulasController()
        {
            formulaAnalizer = new FormulaAnalizer();
            formulaGenerator = new FormulaGenerator();
        }

        [HttpGet("ValidateFormula/{formula}")]
        public Response ValidateFormula(string formula)
        {
            Response response = new Response()
            {
                Success = false
            };

            if (String.IsNullOrWhiteSpace(formula))
            {
                response.Error = "formula is empty";

                return response;
            }
            formula = formula.Replace(" ", "");

            try
            {
                var revitFunctions = formulaAnalizer.Analize(formula);

                if (revitFunctions == null)
                {
                    response.Error = "NULL error";

                    return response;
                }
                response.Content = JsonConvert.SerializeObject(revitFunctions);

                return response;
            }
            catch (Exception ex)
            {
                response.Error = $"Exception: {ex.Message}";

                return response;
            }
        }

        [HttpPost("GenerateFormula")]
        public string GenerateFormula([FromBody] List<RevitFunctionViewModel> revitFunctions)
        {
            if (revitFunctions == null || !revitFunctions.Any())
            {
                throw new Exception("Request body is empty");
            }

            var formula = formulaGenerator.Generate(revitFunctions);

            return formula;
        }
    }
}
