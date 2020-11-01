using RevitFormulasValidator.Data;
using RevitFormulasValidator.Enums;
using RevitFormulasValidator.Extensions;
using RevitFormulasValidator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RevitFormulasValidator.FormulasWorkers
{
    public class FormulaAnalizer
    {
        private readonly char openingBracket = '(';
        private readonly char closingBracket = ')';
        private readonly char comma = ',';

        public List<RevitFunctionModel> Analize(string formula)
        {
            if (!formula.Contains(openingBracket) || !formula.Contains(closingBracket))
            {
                var function = new RevitFunctionModel(RevitEnums.FunctionType.SimpleFunction);
                function.AddArgument(formula);

                return new List<RevitFunctionModel>()
                {
                    function
                };
            }

            List<RevitFunctionModel> revitFunctionsInFormula = GetFunctionsInFormula(formula);
            List<RevitFunctionsPositionModel> bracketsPosList = GetBracketsPosList(formula);

            CountCommas(formula, revitFunctionsInFormula);

            return AssignFunctions(bracketsPosList, revitFunctionsInFormula);
        }

        private void CountCommas(string formula, List<RevitFunctionModel> revitFunctionsInFormula)
        {
            foreach (var function in revitFunctionsInFormula)
            {
                int counter = 1;
                for (int i = function.StartPos + 1; i < formula.Length; i++)
                {
                    if (formula[i].Equals(openingBracket))
                    {
                        counter++;
                    }
                    else if (formula[i].Equals(closingBracket))
                    {
                        counter--;
                    }

                    if (counter == 0)
                    {
                        CheckCommasInFunction(function, formula, i);
                        break;
                    }
                }
            }
        }

        private void CheckCommasInFunction(RevitFunctionModel function, string formula, int functionEndPos)
        {
            var numOfArgs = FunctionTypeAnalizer.CheckNumberOfArgs(function.FunctionType);
            if (numOfArgs > 1)
            {
                bool insideChildFunction = false;
                int childFunctionCounter = 0;
                var correctNumOfCommas = numOfArgs - 1;
                int numOfCommas = 0;
                for (int i = function.StartPos + 1; i < functionEndPos; i++)
                {
                    char character = formula[i];
                    if (character.Equals(openingBracket))
                    {
                        if (childFunctionCounter == 0)
                        {
                            insideChildFunction = true;
                        }
                        childFunctionCounter++;
                    }
                    else if (character.Equals(closingBracket))
                    {
                        childFunctionCounter--;
                        if (childFunctionCounter == 0)
                        {
                            insideChildFunction = false;
                        }
                    }

                    if (!insideChildFunction)
                    {
                        if (character.Equals(comma))
                        {
                            numOfCommas++;
                        }
                    }

                    if (correctNumOfCommas < numOfCommas)
                    {
                        throw new Exception($"Unnecessary comma in formula at index {i} - possibly bracket around this index.");
                    }
                }
                if (correctNumOfCommas > numOfCommas)
                {
                    throw new Exception(
                        $"Missing commas in {function.FunctionTypeName} starting at index {function.StartPos}.\nFunction should have {correctNumOfCommas} commas but contains only {numOfCommas}.");
                }
            }
        }

        private List<RevitFunctionModel> AssignFunctions(List<RevitFunctionsPositionModel> functionsTypeList, List<RevitFunctionModel> revitFunctionsInFormula)
        {
            int iterator = 0;
            int actualParentFunctionNum = -1;
            List<RevitFunctionModel> revitFunctions = new List<RevitFunctionModel>();
            RevitFunctionModel function = null, parentFunction = null;
            foreach (RevitFunctionsPositionModel functionPos in functionsTypeList)
            {
                // if(heigh=15, 145, if(and(a>15, b<16), 245,deep))
                // if(heigh=15, 145, if(and(a>15, b<16), and( ),deep))
                // if(if(and(a>15, b<16), and(4,5),deep), 145,and(heigh=15,h=2) )
                if (functionPos.Stage == RevitEnums.FunctionStage.Open)
                {
                    if (actualParentFunctionNum < iterator)
                    {
                        parentFunction = function;
                    }
                    else if (actualParentFunctionNum > iterator)
                    {
                        parentFunction = parentFunction.ParentFunction;
                    }
                    actualParentFunctionNum = iterator;
                    function = revitFunctionsInFormula.First(fun => fun.StartPos == functionPos.Position);

                    if (actualParentFunctionNum == 0)
                    {
                        revitFunctions.Add(function);
                    }
                    else
                    {
                        parentFunction.AddChildFunction(function);
                    }

                    iterator++;
                }
                else if (functionPos.Stage == RevitEnums.FunctionStage.Close)
                {
                    iterator--;
                }
            }

            return revitFunctions;
        }

        private List<RevitFunctionModel> GetFunctionsInFormula(string formula)
        {
            var functionsList = new List<RevitFunctionModel>();
            for (int i = 0; i < formula.Length; i++)
            {
                char character = formula[i];
                if (character.Equals(openingBracket))
                {
                    RevitFunctionModel revitFunction = null;
                    foreach (RevitEnums.FunctionType revitEnum in Enum.GetValues(typeof(RevitEnums.FunctionType)))
                    {
                        var descLenght = revitEnum.GetDescription().Length;
                        var startIndex = i - descLenght;
                        if (startIndex < 0)
                        {
                            startIndex = 0;
                        }
                        if (descLenght < formula.Length)
                        {
                            string function = formula.Substring(startIndex, descLenght);
                            bool isContaining = Regex.IsMatch(function, $@"\b{revitEnum.GetDescription()}\b", RegexOptions.IgnoreCase);
                            if (isContaining)
                            {
                                revitFunction = new RevitFunctionModel(revitEnum)
                                {
                                    StartPos = i
                                };
                                functionsList.Add(revitFunction);
                            }
                        }
                    }
                    if (revitFunction == null)
                    {
                        throw new Exception($"Opening bracket without known function at index {i}.");
                    }
                }
            }

            return functionsList;
        }

        private List<RevitFunctionsPositionModel> GetBracketsPosList(string formula)
        {
            List<RevitFunctionsPositionModel> bracketsPosList = new List<RevitFunctionsPositionModel>();
            for (int i = 0; i < formula.Length; i++)
            {
                char character = formula[i];
                if (character.Equals(openingBracket))
                {
                    bracketsPosList.Add(new RevitFunctionsPositionModel()
                    {
                        Position = i,
                        Stage = RevitEnums.FunctionStage.Open
                    });
                }
                else if (character.Equals(closingBracket))
                {
                    bracketsPosList.Add(new RevitFunctionsPositionModel()
                    {
                        Position = i,
                        Stage = RevitEnums.FunctionStage.Close
                    });
                }
            }

            if (bracketsPosList.Count % 2 != 0)
            {
                throw new Exception($"Odd number of brackets. Count them!.");
            }

            return bracketsPosList;
        }
    }
}
