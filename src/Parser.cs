using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Compiler
{
    class Parser
    {
        private const string FILE_PATH = "input/test.txt";
        private const string RESULT_OUTPUT_PATH = "output/output.txt";

        private static int ParseIndex { get; set; }
        private static string PrevWord { get; set; }
        private static string CurrentLine { get; set; }

        private List<char> Input = new List<char>();
        private TableMaker TMaker { get; set; }
        private SymbolTable STable { get; set; }

        private static List<char> StoppingChar = new List<char>
        {
            ' ',
            '+',
            '-',
            '=',
            '*',
            '^',
            '/',
            '(',
            ')',
            ',',
            '{',
            '}',
            '\n',
            '\r'
        };

        public Parser()
        {
            if (File.Exists(RESULT_OUTPUT_PATH)) File.Delete(RESULT_OUTPUT_PATH);

            TMaker = new TableMaker();
            STable = new SymbolTable();
            ErrMod.ResetErrMod(); 
            TakeInput();

            ParseIndex = 0;

            while (ParseIndex < Input.Count)
            {
                using StreamWriter file = new(RESULT_OUTPUT_PATH, append: true);

                if (Input[ParseIndex] == '\n')
                {
                    ParseIndex++;
                    continue;
                }

                if (Parse())
                {
                    int iTestAnswer;
                    float fTestAnswer;

                    IR ir = new IR(CurrentLine);
                    AssignVariables(ir);
                    file.WriteLine($"{CurrentLine.PadRight(55)} is valid.");
                    if (int.TryParse(ir.Answer, out iTestAnswer))
                    {
                        file.WriteLine($"The answer is {ir.Answer}\nThe postfix IR is {ir.PostFixString}");
                    }
                    else if (float.TryParse(ir.Answer, out fTestAnswer))
                    {
                        file.WriteLine($"The answer is {ir.Answer}\nThe postfix IR is {ir.PostFixString}");
                    }

                    file.Write("\n");
                    PrevWord = null;
                }
                else
                {
                    file.WriteLine($"{CurrentLine.PadRight(55)} is invalid.\n");
                    PrevWord = null;
                }

                CurrentLine = "";
            }
        }

        private bool Parse()
        {
            Stack<string> parseStack = new Stack<string>();
            string word = NextWord();

            parseStack.Push("eof");
            parseStack.Push("Goal");

            string focus = parseStack.Peek();

            while (true)
            {
                if (focus == "eof" && word == "eof")    // end of file
                {
                    return true;
                }
                else if (TableMaker.Terminals.Contains(focus.ToString()) || focus == "eof")   // focus is terminal
                {
                    if (focus == word)
                    {
                        parseStack.Pop();
                        word = NextWord();
                    }
                    else
                    {
                        return false;
                    }
                }
                else    // focus is nonterminal
                {
                    if (TMaker.Table[focus][word] != -1)
                    {
                        parseStack.Pop();

                        var production = TMaker.Language.Productions[TMaker.Table[focus][word]];

                        for (int i = production.Count - 1; i != 0; i--)
                        {
                            if (production[i] != "@")
                            {
                                parseStack.Push(production[i]);
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                focus = parseStack.Peek();
            }
        }

        private void TakeInput()
        {
            using (StreamReader reader = new StreamReader(FILE_PATH))
            {
                while (reader.Peek() >= 0)
                {
                    Input.Add((char)reader.Read());
                }
            }
        }

        private string NextWord()
        {
            if (ParseIndex >= Input.Count) return "eof";

            while (Input[ParseIndex] == ' ')
            {
                ParseIndex++;
            }

            if (Input[ParseIndex] == '\n' || Input[ParseIndex] == '\r')
            {
                ParseIndex++;
                return "eof";
            }

            StringBuilder builder = new StringBuilder();

            do
            {
                while (Input[ParseIndex] == ' ')
                {
                    ParseIndex++;
                }
                builder.Append(Input[ParseIndex]);
                ParseIndex++;

                if (TableMaker.Terminals.Contains(builder.ToString())) break;

                if (ParseIndex >= Input.Count) break;
            }
            while (!StoppingChar.Contains(Input[ParseIndex]));

            if (builder.ToString() == "-" &&
            !(PrevWord == "name" || PrevWord == "num_value" || PrevWord == "ish_value" || PrevWord == ")"
            || PrevWord == "spacenegname" || PrevWord == "spacenegnum_value" || PrevWord == "spacenegish_value"))
            {
                do
                {
                    while (Input[ParseIndex] == ' ')
                    {
                        ParseIndex++;
                    }
                    builder.Append(Input[ParseIndex]);
                    ParseIndex++;

                    if (ParseIndex >= Input.Count) break;
                }
                while (!StoppingChar.Contains(Input[ParseIndex]));
            }

            string builtString = builder.ToString();

            int outInt = 0;
            float outFloat = 0;
            if (Int32.TryParse(builtString, out outInt))
            {
                if (outInt < 0) // neg number
                {
                    PrevWord = "spacenegnum_value";
                }
                else
                {
                    PrevWord = "num_value";
                }
            }
            else if (float.TryParse(builtString, out outFloat))
            {
                if (outInt < 0) // neg number
                {
                    PrevWord = "spacenegish_value";
                }
                else
                {
                    PrevWord = "ish_value";
                }
            }
            else if (!TableMaker.Terminals.Contains(builtString))
            {
                if (builtString.StartsWith("-")) // neg var
                {
                    PrevWord = "spacenegname";
                }
                else
                {
                    PrevWord = "name";
                }
            }
            else
            {
                PrevWord = builtString;
            }

            CurrentLine += (builtString + " ");
            return PrevWord;
        }

        private void AssignVariables(IR ir)
        {
            if (CurrentLine.Contains("="))
            {
                string[] line = CurrentLine.Split("=");
                string[] lhs = line[0].Split(' ');

                // if declaring variable (num var = 1)
                if (lhs.Length > 1)
                {
                    string varType = lhs[0];
                    string varName = lhs[1];

                    if (SymbolTable.IsVarDefined(varName))
                    {
                        // var name cannot be reused
                        ErrMod.ThrowError($"A variable called {varName} already exists in this instance.");
                    }
                    else
                    {
                        STable.Insert(Variable.DecideVarType(varType), varName, ir);
                    }
                }
                // if assigning variable (var = 1)
                else
                {
                    STable.Update(lhs[0], ir);
                }
            }
        }
    }
}