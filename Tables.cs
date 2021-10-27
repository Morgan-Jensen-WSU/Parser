using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace parser
{
    class TableMaker
    {

        private const string FILE_PATH = "input/valid_simple.txt";

        private static int ParseIndex { get; set; }

        private List<char> Input = new List<char>();

        private Stack<string> ParseStack = new Stack<string>();

        private static Dictionary<int, List<string>> Productions = new Dictionary<int, List<string>>();
        private static Dictionary<string, List<string>> First = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> Follow = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> FirstPlus = new Dictionary<string, List<string>>();

        private static Dictionary<string, Dictionary<string, int>> Table = new Dictionary<string, Dictionary<string, int>>();


        private static List<string> Terminals = new List<string>
        {
            "num",
            "name",
            "(",
            ")",
            "+",
            "-",
            "x",
            "/",
            "eof",
            "e"
        };
        private static List<string> NonTerminals = new List<string>
        {
            "Goal",
            "Expr",
            "LTerm",
            "Expr'",
            "Term",
            "Term'",
            "LFactor",
            "RFactor"
        };

        private static List<char> StoppingChar = new List<char>
        {
            ' ',
            '+',
            '-',
            '=',
            '*',
            '/',
            '(',
            ')',
            '\n',
            '\r'
        };

        public TableMaker()
        {
            TakeInput();
            // FillProduction();
            FillSampleProduction();
            FillTable();

            ParseIndex = 0;

            if (Parse())
            {
                Console.WriteLine("Valid");
            }
            else
            {
                Console.WriteLine("Invalid");
            }
        }

        private bool Parse()
        {
            string word = NextWord();

            ParseStack.Push("eof");
            ParseStack.Push("Goal");

            string focus = ParseStack.Peek();

            while (true)
            {
                if (focus == "e")
                {
                    ParseStack.Pop();
                    focus = ParseStack.Peek();
                    continue;
                }

                if (focus == "eof" && word == "eof")    // end of file
                {
                    return true;
                }
                else if (Terminals.Contains(focus.ToString()) || focus == "eof")   // focus is terminal
                {
                    if (focus == word)
                    {
                        ParseStack.Pop();
                        word = NextWord();
                    }
                    else
                    {
                        return false;
                    }
                }
                else    // focus is nonterminal
                {

                    if (Table[focus][word] != -1)
                    {
                        ParseStack.Pop();

                        var production = Productions[Table[focus][word]];

                        for (int i = production.Count - 1; i != -1; i--)
                        {
                            ParseStack.Push(production[i]);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                focus = ParseStack.Peek();
            }

        }

        private void FillSampleProduction()
        {
            Productions[0] = new List<string> 
            { 
                "Expr" 
            };

            Productions[1] = new List<string> 
            { 
                "Term",
                "Expr'" 
            };

            Productions[2] = new List<string> 
            { 
                "+",
                "Term",
                "Expr'" 
            };
            
            Productions[3] = new List<string> 
            { 
                "-",
                "Term",
                "Expr'" 
            };

            Productions[4] = new List<string> 
            { 
                "e" 
            };

            Productions[5] = new List<string> 
            { 
                "Factor",
                "Term'" 
            };

            Productions[6] = new List<string> 
            { 
                "*",
                "Factor",
                "Term'" 
            };

            Productions[7] = new List<string> 
            { 
                "/",
                "Factor",
                "Term'"
            };

            Productions[8] = new List<string> 
            { 
                "e" 
            };

            Productions[9] = new List<string> 
            { 
                "(",
                "Expr",
                ")"
            };

            Productions[10] = new List<string> 
            { 
                "num"
            };

            Productions[11] = new List<string> 
            { 
                "name" 
            };
        }

        private void FillProduction()
        {
            // Goal -> Expr
            Productions[0] = new List<string> 
            { 
                "Goal",
                "Expr" 
            };

            // Expr -> LTerm Expr'
            Productions[1] = new List<string>
            {
                "Expr",
                "LTerm",
                "Expr'"
            };

            // LTerm -> LFactor Expr'
            Productions[2] = new List<string>
            {
                "LTerm",
                "LFactor",
                "Expr'"
            };

            // Expr' -> + Term Expr'
            Productions[3] = new List<string>
            {
                "Expr'",
                "+",
                "Term",
                "Expr'"                
            };

            // Expr' -> - Term Expr'
            Productions[4] = new List<string>
            {
                "Expr'",
                "-",
                "Term",
                "Expr'"
            };

            // Expr' -> e
            Productions[5] = new List<string> 
            { 
                "Expr'",
                "e" 
            };

            // Term -> RFactor Term'
            Productions[6] = new List<string>
            {
                "Term",
                "RFactor",
                "Term'"
            };

            // Term' -> * Term Expr'
            Productions[7] = new List<string>
            {
                "Term'",
                "*",
                "Term",
                "Expr'"
            };

            // Term' -> / Term Expr'
            Productions[8] = new List<string>
            {
                "Term'",
                "/",
                "Term",
                "Expr'"
            };

            // Term' -> e
            Productions[9] = new List<string> 
            { 
                "Term'",
                "e" 
            };

            // LFactor -> (Expr)
            Productions[10] = new List<string>
            {
                "LFactor",
                "(",
                "Expr",
                ")"
            };

            // LFactor -> num
            Productions[11] = new List<string> 
            { 
                "LFactor",
                "num" 
            };

            // LFactor -> - num
            Productions[12] = new List<string>
            {
                "LFactor",
                "-",
                "num"
            };

            // LFactor -> name
            Productions[13] = new List<string> 
            { 
                "LFactor",
                "name" 
            };

            // LFactor -> - name
            Productions[14] = new List<string>
            {
                "LFactor",
                "-",
                "name"
            };

            // RFactor -> (Expr)
            Productions[15] = new List<string>
            {
                "RFactor",
                "(",
                "Expr",
                ")"
            };

            // RFactor -> num
            Productions[16] = new List<string> 
            { 
                "RFactor",
                "num" 
            };

            // RFactor -> [space] - num
            Productions[17] = new List<string>
            {
                "RFactor",
                " ",
                "-",
                "num"
            };

            // RFactor -> name
            Productions[18] = new List<string> 
            { 
                "RFactor",
                "name" 
            };

            // RFactor -> [space] - name
            Productions[19] = new List<string>
            {
                "RFactor",
                " ",
                "-",
                "name"
            };
        }

        private void FillTable()
        {
            Table["Goal"] = new Dictionary<string, int>()
            {
                { "eof",   -1 },
                { "+",     -1 },
                { "-",     -1 },
                { "*",     -1 },
                { "/",     -1 },
                { "(",      0 },
                { ")",     -1 },
                { "name",   0 },
                { "num",    0 }
            };

            Table["Expr"] = new Dictionary<string, int>()
            {
                { "eof",   -1 },
                { "+",     -1 },
                { "-",     -1 },
                { "*",     -1 },
                { "/",     -1 },
                { "(",      1 },
                { ")",     -1 },
                { "name",   1 },
                { "num",    1 }
            };

            Table["Expr'"] = new Dictionary<string, int>()
            {
                { "eof",    4 },
                { "+",      2 },
                { "-",      3 },
                { "*",     -1 },
                { "/",     -1 },
                { "(",     -1 },
                { ")",      4 },
                { "name",  -1 },
                { "num",   -1 }
            };

            Table["Term"] = new Dictionary<string, int>()
            {
                { "eof",    -1 },
                { "+",      -1 },
                { "-",      -1 },
                { "*",      -1 },
                { "/",      -1 },
                { "(",       5 },
                { ")",      -1 },
                { "name",    5 },
                { "num",     5 }
            };

            Table["Term'"] = new Dictionary<string, int>()
            {
                { "eof",     8 },
                { "+",       8 },
                { "-",       8 },
                { "*",       6 },
                { "/",       7 },
                { "(",      -1 },
                { ")",       8 },
                { "name",   -1 },
                { "num",    -1 }
            };

            Table["Factor"] = new Dictionary<string, int>()
            {
                { "eof",    -1 },
                { "+",      -1 },
                { "-",      -1 },
                { "*",      -1 },
                { "/",      -1 },
                { "(",       9 },
                { ")",      -1 },
                { "name",   11 },
                { "num",    10 }
            };
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

            StringBuilder builder = new StringBuilder();

            do
            {
                builder.Append(Input[ParseIndex]);
                ParseIndex++;
            }
            while (!StoppingChar.Contains(Input[ParseIndex]));
            ParseIndex++;

            string builtString = builder.ToString();

            int temp = 0;
            if (Int32.TryParse(builtString, out temp))
            {
                return "num";
            }
            else if (!Terminals.Contains(builtString))
            {
                return "name";
            }
            else
            {
                return builtString;
            }
        }
    }
}