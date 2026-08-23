using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model
{
    public class FormulaModel : Model<FormulaModel>
    {
        private const string VARIABLE_PATTERN = @"\$\{(\w+)\.(\w+)\}";
        private const string NUMBER_PATTERN = @"(?<![a-zA-Z_\d}])-?\d*\.?\d+";
        private readonly Dictionary<string, Func<double, double, double>> Operators = new()
        {
            {"+", (a, b) => a + b},
            {"-", (a, b) => a - b},
            {"*", (a, b) => a * b},
            {"/", (a, b) => a / b},
            {"%", (a, b) => a % b}
        };

        public string Formula { get; set; }
        private Dictionary<string, double> Variables { get; set; } = new();

        public (Dictionary<string, string> Variables, HashSet<double> Numbers) ExtractElements()
        {
            var cleanFormula = RemoveWhitespace(Formula);
            return (
                ExtractVariables(cleanFormula),
                ExtractNumbers(cleanFormula)
            );
        }

        private static string RemoveWhitespace(string input)
        {
            return Regex.Replace(input, @"\s+", "");
        }

        private static Dictionary<string, string> ExtractVariables(string formula)
        {
            var variables = new Dictionary<string, string>();
            var matches = Regex.Matches(formula, VARIABLE_PATTERN);

            foreach (Match match in matches)
            {                
                variables.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
            return variables;
        }

        private static HashSet<double> ExtractNumbers(string formula)
        {
            var numbers = new HashSet<double>();
            var matches = Regex.Matches(formula, NUMBER_PATTERN);

            foreach (Match match in matches)
            {
                if (double.TryParse(match.Value, out double number))
                {
                    numbers.Add(number);
                }
            }
            return numbers;
        }

        public double EvaluateFormula(Dictionary<string, double> variableValues)
        {
            Variables = variableValues;
            var processedFormula = RemoveWhitespace(Formula);

            ValidateVariables(processedFormula, variableValues);
            processedFormula = ReplaceVariables(processedFormula, variableValues);

            return EvaluateExpression(processedFormula);
        }

        private void ValidateVariables(string formula, Dictionary<string, double> variableValues)
        {
            var variables = ExtractVariables(formula);
            var missingVariables = variables.Where(v => !variableValues.ContainsKey($"{v.Key}.{v.Value}")).ToList();

            if (missingVariables.Any())
            {
                throw new ArgumentException(
                    $"缺少以下變量的值: {string.Join(", ", missingVariables)}"
                );
            }
        }

        private static string ReplaceVariables(string formula, Dictionary<string, double> variableValues)
        {
            foreach (var variable in variableValues)
            {
                formula = Regex.Replace(
                    formula,
                    $@"\$\{{{variable.Key}\}}",
                    variable.Value.ToString()
                );
            }
            return formula;
        }

        private double EvaluateExpression(string expression)
        {
            // 处理括号
            while (expression.Contains("("))
            {
                expression = Regex.Replace(expression, @"\(([^()]+)\)", match =>
                {
                    string innerExpression = match.Groups[1].Value;
                    double result = CalculateSimpleExpression(innerExpression);
                    return result.ToString();
                });
            }

            return CalculateSimpleExpression(expression);
        }

        private double CalculateSimpleExpression(string expression)
        {
            var tokens = TokenizeExpression(expression);

            // 按运算符优先级计算
            ProcessOperations(tokens, new[] { "*", "/", "%" });
            ProcessOperations(tokens, new[] { "+", "-" });

            return double.Parse(tokens[0]);
        }

        private List<string> TokenizeExpression(string expression)
        {
            return Regex.Matches(expression, @"(-?\d*\.?\d+)|([+\-*/%])")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
        }

        private void ProcessOperations(List<string> tokens, string[] operations)
        {
            for (int i = 1; i < tokens.Count - 1;)
            {
                if (operations.Contains(tokens[i]))
                {
                    double left = double.Parse(tokens[i - 1]);
                    double right = double.Parse(tokens[i + 1]);

                    if (Operators.TryGetValue(tokens[i], out var operation))
                    {
                        tokens[i - 1] = operation(left, right).ToString();
                        tokens.RemoveRange(i, 2);
                    }
                }
                else
                {
                    i += 2;
                }
            }
        }
    }
}