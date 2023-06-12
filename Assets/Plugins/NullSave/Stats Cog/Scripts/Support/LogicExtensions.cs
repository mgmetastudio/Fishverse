using System.Globalization;
using UnityEngine;

namespace NullSave.TOCK.Stats
{

    public static class LogicExtensions
    {

        #region Constants

        public const string BASIC_EXPRESSIONS = "=+-*/^";
        public const string FULL_EXPRESSIONS = "=+-*/^<>floor()ceil()abs()rnd_f()rnd_i()";

        #endregion

        #region Public Methods

        public static float EvaluateSimpleMath(this string equation)
        {
            try
            {
                int open, close;
                string exp;
                float mid;

                equation = equation.Replace(" ", string.Empty);
                if (string.IsNullOrEmpty(equation)) return 0;

                // Parentheses
                while (true)
                {
                    close = equation.IndexOf(')');
                    if (close > -1)
                    {
                        open = equation.LastIndexOf('(', close, close + 1);
                        exp = equation.Substring(open + 1, close - open - 1);

                        bool isFloor = open >= 5 && equation.Substring(open - 5, 5).ToLower() == "floor";
                        bool isCeil = open >= 4 && equation.Substring(open - 4, 4).ToLower() == "ceil";
                        bool isAbs = open >= 3 && equation.Substring(open - 3, 3).ToLower() == "abs";
                        bool isRndI = open >= 5 && equation.Substring(open - 5, 5).ToLower() == "rnd_i";
                        bool isRndF = open >= 5 && equation.Substring(open - 5, 5).ToLower() == "rnd_f";

                        mid = exp.EvaluateSimpleMath();
                        if (isFloor)
                        {
                            mid = Mathf.Floor(mid);
                            open -= 5;
                        }
                        else if (isCeil)
                        {
                            mid = Mathf.Ceil(mid);
                            open -= 4;
                        }
                        else if (isAbs)
                        {
                            mid = Mathf.Abs(mid);
                            open -= 3;
                        }
                        else if(isRndF)
                        {
                            mid = Random.Range(0, mid);
                            open -= 5;
                        }
                        else if(isRndI)
                        {
                            mid = Random.Range(0, (int)mid);
                            open -= 5;
                        }

                        equation = equation.Substring(0, open) + mid.ToString(CultureInfo.InvariantCulture) + equation.Substring(close + 1);
                    }
                    else break;
                }

                // Handle E-/+
                equation = equation.Replace("E-", string.Empty + (char)1);
                equation = equation.Replace("E+", string.Empty + (char)2);

                // Exponents
                equation = HandlePEMDAS(equation, '^');

                // Multiplication
                equation = HandlePEMDAS(equation, '*');

                // Division
                equation = HandlePEMDAS(equation, '/');

                // Addition
                equation = HandlePEMDAS(equation, '+');

                // Subtraction
                equation = HandlePEMDAS(equation, '-');

                return float.Parse(equation, CultureInfo.InvariantCulture);
            }
            catch
            {
                Debug.LogError("Could not parse '" + equation + "'");
                throw;
            }
        }

        public static string RemoveMathSymbols(this string value)
        {
            return value.Replace("(", string.Empty).Replace(")", string.Empty).Replace("/", string.Empty).Replace("+", string.Empty).Replace("-", string.Empty).Replace("*", string.Empty).Replace("^", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("=", string.Empty).Replace(" ", string.Empty);
        }

        #endregion

        #region Private Methods

        private static string HandlePEMDAS(string equation, char op)
        {
            int opLoc;
            int left, right;
            string res = string.Empty;

            while (true)
            {
                // Find sides
                opLoc = equation.IndexOf(op, 1);
                if (opLoc == -1) return equation;
                left = LeftValueStart(equation, opLoc);
                right = RightValueStart(equation, opLoc);

                try
                {
                    // Perform math
                    switch (op)
                    {
                        case '^':
                            res = Mathf.Pow(ParseToFloat(equation, left, opLoc - left), ParseToFloat(equation, opLoc + 1, right - opLoc)).ToString(CultureInfo.InvariantCulture);
                            break;
                        case '*':
                            res = (ParseToFloat(equation, left, opLoc - left) * ParseToFloat(equation, opLoc + 1, right - opLoc)).ToString(CultureInfo.InvariantCulture);
                            break;
                        case '/':
                            res = (ParseToFloat(equation, left, opLoc - left) / ParseToFloat(equation, opLoc + 1, right - opLoc)).ToString(CultureInfo.InvariantCulture);
                            break;
                        case '+':
                            res = (ParseToFloat(equation, left, opLoc - left) + ParseToFloat(equation, opLoc + 1, right - opLoc)).ToString(CultureInfo.InvariantCulture);
                            break;
                        case '-':
                            res = (ParseToFloat(equation, left, opLoc - left) - ParseToFloat(equation, opLoc + 1, right - opLoc)).ToString(CultureInfo.InvariantCulture);
                            break;
                    }
                }
                catch
                {
                    Debug.LogError("Cannot parse '" + equation + "'");
                    throw;
                }

                // Replace in equation
                equation = equation.Replace(equation.Substring(left, right - left + 1), res);
            }
        }

        private static int LeftValueStart(string equation, int opLoc)
        {
            int startFrom = opLoc - 1;

            while (startFrom > 0)
            {
                if (BASIC_EXPRESSIONS.Contains(equation.Substring(startFrom, 1)))
                {
                    startFrom += 1;
                    break;
                }
                startFrom -= 1;
            }

            return startFrom;
        }

        private static int RightValueStart(string equation, int opLoc)
        {
            int startFrom = opLoc + 1;

            while (startFrom < equation.Length - 1)
            {
                if (BASIC_EXPRESSIONS.Contains(equation.Substring(startFrom, 1)))
                {
                    if (equation[startFrom] == '-' && 
                        ( equation[startFrom - 1] == '+' || equation[startFrom - 1] == '*' || equation[startFrom - 1] == '/'))
                    { 
                        // Allow
                    }
                    else if (equation[startFrom] != '-' || equation[startFrom - 1] != '-')
                    {
                        startFrom -= 1;
                        break;
                    }
                }
                startFrom += 1;
            }

            return startFrom;
        }

        private static float ParseToFloat(string equation, int start, int length)
        {
            return float.Parse(equation.Substring(start, length).Replace(string.Empty + (char)1, "E-").Replace(string.Empty + (char)2, "E+"), CultureInfo.InvariantCulture);
        }

        #endregion

    }
}