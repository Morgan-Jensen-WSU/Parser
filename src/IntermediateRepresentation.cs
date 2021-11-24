using System;
using System.Collections.Generic;

namespace Compiler
{
    /// <summary>
    /// Uses the "Shunting Yard" algorithm to calculate the answer
    /// </summary>
    public class IR
    {
        public string Answer { get; set; }
        public string PostFixString { get; set; }

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

            return OpPrecedence[op] < OpPrecedence[OperatorStack.Peek()];
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
                flA = float.Parse(SymbolTable.Lookup(a).Rep.Answer);
            }
            try
            {
                flB = float.Parse(b);
            }
            catch
            {
                flB = float.Parse(SymbolTable.Lookup(b).Rep.Answer);
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

            string retVal = OutputStack.Pop();

            if (SymbolTable.IsVarDefined(retVal))
            {
                return SymbolTable.Lookup(retVal).Rep.Answer;
            }
            return retVal;
        }
    }
}