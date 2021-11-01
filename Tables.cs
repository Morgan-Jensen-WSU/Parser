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
            "negnum",
            "negname",
            "spacenegnum",
            "spacenegname",
            "(",
            ")",
            "+",
            "-",
            "*",
            "/"
        };

        private static List<string> NonTerminals = new List<string>
        {
            "Goal",
            "Expr",
            "LTerm",
            "RTerm",
            "Expr'",
            "Term'",
            "LFactor",
            "RFactor",
            "GFactor",
            "PosVal",
            "SpaceNegVal"
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
            FillProduction();
            // FillSampleProduction();
            // FillSampleTable();
            FillTable();

            ParseIndex = 0;
            
            // while (ParseIndex < Input.Count)
            // {
            //     if (Parse())
            //     {
            //         Console.WriteLine("Valid");
            //     }
            //     else
            //     {
            //         Console.WriteLine("Invalid");
            //     }
            // }
        }

        private bool Parse()
        {
            string word = NextWord();

            ParseStack.Push("eof");
            ParseStack.Push("Goal");

            string focus = ParseStack.Peek();

            while (true)
            {
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
                            if (production[i] != "e")
                            {
                                ParseStack.Push(production[i]);
                            }
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

            Productions[3] = new List<string>
            {
                "RTerm",
                "RFactor",
                "Term'"
            };

            // Expr' -> + RTerm Expr'
            Productions[4] = new List<string>
            {
                "Expr'",
                "+",
                "RTerm",
                "Expr'"                
            };

            // Expr' -> - RTerm Expr'
            Productions[5] = new List<string>
            {
                "Expr'",
                "-",
                "RTerm",
                "Expr'"
            };

            // Expr' -> e
            Productions[6] = new List<string> 
            { 
                "Expr'",
                "e" 
            };

            // Term' -> * RTerm Expr'
            Productions[7] = new List<string>
            {
                "Term'",
                "*",
                "RTerm",
                "Expr'"
            };

            // Term' -> / RTerm Expr'
            Productions[8] = new List<string>
            {
                "Term'",
                "/",
                "RTerm",
                "Expr'"
            };

            // Term' -> e
            Productions[9] = new List<string> 
            { 
                "Term'",
                "e" 
            };

            // LFactor -> GFactor
            Productions[10] = new List<string>
            {
                "LFactor",
                "GFactor"
            };

            // LFactor -> negnum
            Productions[11] = new List<string> 
            { 
                "LFactor",
                "negnum" 
            };

            // LFactor -> negname
            Productions[12] = new List<string>
            {
                "LFactor",
                "negname"
            };

            // RFactor -> GFactor
            Productions[13] = new List<string>
            {
                "RFactor",
                "GFactor"
            };

            // GFactor -> ( Expr )
            Productions[14] = new List<string>
            {
                "GFactor",
                "(",
                "Expr",
                ")"
            };

            // GFactor -> PosVal
            Productions[15] = new List<string>
            {
                "GFactor",
                "PosVal"
            };

            // GFactor -> SpaceNegVal
            Productions[16] = new List<string>
            {
                "GFactor",
                "SpaceNegVal"
            };

            // PosVal -> num
            Productions[17] = new List<string>
            {
                "PosVal",
                "num"
            };

            // PosVal -> name
            Productions[18] = new List<string>
            {
                "PosVal",
                "name"
            };

            // SpaceNegVal -> spacenegnum
            Productions[19] = new List<string>
            {
                "SpaceNegVal",
                "spacenegnum"
            };

            // SpaceNegVal -> spacenegname
            Productions[20] = new List<string>
            {
                "SpaceNegVal",
                "spacenegname"
            };

        }

        private void FillSampleTable()
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

        private void FillTable()
        {
            BuildFirst();
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

            if (Input[ParseIndex] == ' ')
            {
                ParseIndex++;
            }
            else if (Input[ParseIndex] == '\n' || Input[ParseIndex] == '\r')
            {
                ParseIndex++;
                return "eof";
            }

            StringBuilder builder = new StringBuilder();

            do
            {
                builder.Append(Input[ParseIndex]);
                ParseIndex++;
                
                if (Terminals.Contains(builder.ToString())) break;

                if (ParseIndex >= Input.Count) break;
            }
            while (!StoppingChar.Contains(Input[ParseIndex]));

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

        private void BuildFirst()
        {
            // for each t in (T U eof U e) First(t) <- t
            foreach (var t in Terminals)
            {
                First[t] = new List<string>(){ t };
            }
            First["e"] = new List<string> { "e" };
            First["eof"] = new List<string> { "eof" };

            // for each nt in (NT) First(nt) <- empty set
            foreach (var nt in NonTerminals)
            {
                First[nt] = new List<string>();
            }

            bool isChanging = true;
            
            while (isChanging)
            {
                isChanging = false;

                foreach (var prod in Productions)
                {
                    List<string> rhs = new List<string>();
                    List<string> b = prod.Value;
                    int k = b.Count - 1;

                    int checker = First[b[0]].Count;
                    
                    foreach (var firstB in First[b[1]])
                    {
                        if (firstB != "e")
                        {
                            rhs.Add(firstB);
                        }
                    }

                    int i = 1;
                    while (First[b[i]].Contains("e") && i <= k - 1)
                    {
                        // rhs <- rhs U (FIRST(Bi + 1) - "e")
                        foreach(var firstB1 in First[b[i+1]])
                        {
                            if (firstB1 != "e")
                            {
                                rhs.Add(firstB1);
                            }
                        }
                        i += 1;
                    }

                    if (i == k && First[b[k]].Contains("e")) 
                    {
                        rhs.Add("e");
                    }

                    foreach (var val in rhs)
                    {
                        if (!First[b[0]].Contains(val))
                        {
                            First[b[0]].Add(val);
                        }
                    }
                    
                    if (!isChanging)
                    {
                        if (checker != First[b[0]].Count)
                        {
                            isChanging = true;
                        }
                    }
                }
            }

        }
    }
}