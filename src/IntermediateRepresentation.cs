using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    /// <summary>
    /// Uses the "Shunting Yard" algorithm to calculate the answer
    /// </summary>
    public class IR
    {
        public string Answer { get; set; }
        public bool HasAnswer { get; set; } = false;
        public string PostFixString { get; set; }
        public bool IsPrintString { get; set; } = false;
        public bool IsPrintNum { get; set; } = false;
        public bool IsPrintIsh { get; set; } = false;

        private List<string> Operators = new List<string>
        {
            "+",
            "-",
            "*",
            "/",
            "^",
            "(",
            ")"
        };

        private Dictionary<string, int> OpPrecedence = new Dictionary<string, int>()
        {
            {"+", 1},
            {"-", 1},
            {"*", 2},
            {"/", 2},
            {"^", 3},
            {"(", 0},
            {")", 0}
        };

        private Stack<string> OperatorStack { get; set; }
        private Stack<string> OutputStack { get; set; }
        private string[] Words { get; set; }

        public IR(string input)
        {
            Words = input.Split(' ');
            PostFixString = "";

            OperatorStack = new Stack<string>();
            OutputStack = new Stack<string>();

            foreach (string word in Words)
            {
                if (word == "") continue;

                if (Operators.Contains(word))
                {
                    while (IsLowerPrecedenceThanTop(word))
                    {
                        if (word == "(")
                        {
                            break;
                        }
                        else if (word == ")")
                        {
                            while (OperatorStack.Peek() != "(" && OperatorStack.Peek() != ")")
                            {
                                Operate();
                            }
                        }
                        else
                        {
                            Operate();
                        }
                    }
                    OperatorStack.Push(word);
                }
                else
                {
                    OutputStack.Push(word);
                    PostFixString += word + " ";
                }
            }

            Answer = CalculateAnswer();
        }

        private bool IsLowerPrecedenceThanTop(string op)
        {
            if (OperatorStack.Count == 0) return false;

            if (OperatorStack.Peek() == "(" || OperatorStack.Peek() == ")") return false;

            return OpPrecedence[op] <= OpPrecedence[OperatorStack.Peek()];
        }

        private void Operate()
        {
            string a = OutputStack.Pop();
            string b = OutputStack.Pop();
            float flA;
            float flB;

            try
            {
                flA = float.Parse(a);
            }
            catch
            { 
                try
                {
                    flA = float.Parse(SymbolTable.Lookup(a).Rep.Answer);
                }
                catch
                {
                    OutputStack.Push(b);
                    OutputStack.Push(a);
                    OperatorStack.Pop();
                    return;
                }
            }
            try
            {
                flB = float.Parse(b);
            }
            catch
            {
                try
                {
                    flB = float.Parse(SymbolTable.Lookup(b).Rep.Answer);
                }
                catch
                {
                    OutputStack.Push(b);
                    OutputStack.Push(a);
                    OperatorStack.Pop();
                    return;
                }
            }

            string op = OperatorStack.Pop();

            PostFixString += op + " ";


            switch (op)
            {
                case "+":
                    OutputStack.Push((flB + flA).ToString());
                    break;
                case "-":
                    OutputStack.Push((flB - flA).ToString());
                    break;
                case "*":
                    OutputStack.Push((flB * flA).ToString());
                    break;
                case "/":
                    if (flA == 0f)
                    {
                        ErrMod.ThrowError("Cannot divide by zero.");
                        Answer = "Error";
                        return;
                    }
                    OutputStack.Push((flB / flA).ToString());
                    break;
                case "^":
                    OutputStack.Push(Math.Pow(flB, flA).ToString());
                    break;
            }
        }

        private string CalculateAnswer()
        {
            while (OperatorStack.Count != 0)
            {
                if (OperatorStack.Peek() == "(" || OperatorStack.Peek() == ")")
                {
                    OperatorStack.Pop(); 
                }
                else
                {
                    Operate();
                }
            }

            if (OutputStack.Count == 0)
            {
                HasAnswer = false;
                return "NONE";
            }   

            if (OutputStack.Peek() == "\"")
            {
                List<string> text = new List<string>();
                IsPrintString = true;
                HasAnswer = true;
                StringBuilder builder = new StringBuilder();
                OutputStack.Pop();
                
                while (OutputStack.Peek() != "\"")
                {
                    text.Add(OutputStack.Pop());
                }

                text.Reverse();

                foreach (var word in text)
                {
                    builder.Append(word);
                    if (word != text[text.Count - 1]) builder.Append(' ');
                }

                return builder.ToString();
            }

            string retVal = OutputStack.Pop();

            if (OutputStack.Count != 0)
            {
                if (OutputStack.Peek() == "printNum")
                {
                    IsPrintNum = true;
                }
                else if (OutputStack.Peek() == "printIsh")
                {
                    IsPrintIsh = true;
                }
            }

            int numTest = 0;
            float ishTest = 0;

            if (SymbolTable.IsVarDefined(retVal))
            {
                HasAnswer = true;
                return SymbolTable.Lookup(retVal).Rep.Answer;
            }
            else if (int.TryParse(retVal, out numTest) || float.TryParse(retVal, out ishTest))
            {
                HasAnswer = true;
            }
            
            return retVal;
        }
    }
}