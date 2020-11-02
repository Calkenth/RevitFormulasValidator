﻿using Microsoft.AspNetCore.Mvc;
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
            if (String.IsNullOrWhiteSpace(formula))
            {
                return new Response()
                {
                    Error = "formula is empty"
                };
            }
            formula = formula.Replace(" ", "");

            try
            {
                var revitFunctions = formulaAnalizer.Analize(formula);

                if (revitFunctions == null)
                {
                    return new Response()
                    {
                        Error = "NULL error"
                    };
                }

                return new Response()
                {
                    Content = JsonConvert.SerializeObject(revitFunctions)
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Error = $"Exception: {ex.Message}"
                };
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
