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

            OperatorStack = new Stack<string>();
            OutputStack = new Stack<string>();

            foreach (string word in Words)
            {
                if (word == "") continue;

                if (Operators.Contains(word))
                {
                    while (IsLowerPrecedenceThanTop(word))
                    {
                        if (word == "(" || word == ")")
                        {
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


            Answer = CalculateAnswer(); 
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

        private string CalculateAnswer()
        {
            // TODO: check for variables
            Stack<string> calcOpStack = new Stack<string>(new Stack<string>(OperatorStack));
            Stack<string> calcOutStack = new Stack<string>(new Stack<string>(OutputStack));

            while (calcOpStack.Count != 0)
            {
                if (calcOpStack.Peek() == "(" || calcOpStack.Peek() == ")")
                {
                    calcOpStack.Pop();
                    continue;
                }

                int a = int.Parse(calcOutStack.Pop());
                int b = int.Parse(calcOutStack.Pop());
                string op = calcOpStack.Pop();

                switch(op)
                {
                    case "+":
                        calcOutStack.Push((b + a).ToString());
                        break;
                    case "-":
                        calcOutStack.Push((b - a).ToString());
                        break;
                    case "*":
                        calcOutStack.Push((b * a).ToString());
                        break;
                    case "/":
                        calcOutStack.Push((b / a).ToString());
                        break;
                    case "^":
                        calcOutStack.Push((b ^ a).ToString());
                        break;
                }
            }

            return calcOutStack.Pop();
        }
    }
}