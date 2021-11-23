using System.Collections.Generic;

namespace Compiler
{
    /// <summary>
    /// Uses the "Shunting Yard" algorithm to calculate the answer
    /// </summary>
    public class IR
    {
        public string Answer { get; set; }

        private List<string> Operators = new List<string>
        {
            "+",
            "-",
            "*",
            "/",
            "^"
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

            foreach (string word in Words)
            {
                if (Operators.Contains(word))
                {
                    while (IsLowerPrecedenceThanTop(word))
                    {
                        if (word == "(" || word == ")")
                        {
                            OperatorStack.Push(word);
                            break;
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
                }
            }


            // TODO: Answer = CalculateAnswer(); 
        }

        private bool IsLowerPrecedenceThanTop(string op)
        {
            if (OperatorStack.Count == 0) return false;

            return OpPrecedence[op] < OpPrecedence[OperatorStack.Peek()];
        }

        private void Operate()
        {
            int a = int.Parse(OutputStack.Pop());
            int b = int.Parse(OutputStack.Pop());
            string op = OperatorStack.Pop();

            switch(op)
            {
                case "+":
                    OutputStack.Push((b + a).ToString());
                    break;
                case "-":
                    OutputStack.Push((b - a).ToString());
                    break;
                case "*":
                    OutputStack.Push((b * a).ToString());
                    break;
                case "/":
                    OutputStack.Push((b / a).ToString());
                    break;
                case "^":
                    OutputStack.Push((b ^ a).ToString());
                    break;
            }
        }
    }
}