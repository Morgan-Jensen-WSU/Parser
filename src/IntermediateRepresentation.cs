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
                            while (OperatorStack.Peek() != "(")
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

            foreach (var op in OperatorStack)
            {
                if (op != "(" && op != ")") PostFixString += op + " ";
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

            float flA = float.Parse(a);
            float flB = float.Parse(b);
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
                    OutputStack.Push((flB / flA).ToString());
                    break;
            }

            if (op == "^")
            {
                int pwrA = int.Parse(a);
                int pwrB = int.Parse(b);

                OutputStack.Push((pwrA + pwrB).ToString());
                PostFixString += op + " ";
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

                string strA = calcOutStack.Pop();
                string strB = calcOutStack.Pop();

                float a = float.Parse(strA);
                float b = float.Parse(strB);
                string op = calcOpStack.Pop();

                switch (op)
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
                }

                if (op == "^")
                {
                    int pwrA = int.Parse(strA);
                    int pwrB = int.Parse(strB);

                    OutputStack.Push((pwrA + pwrB).ToString());
                    PostFixString += op + " ";
                }
            }
            return calcOutStack.Pop();
        }
    }
}