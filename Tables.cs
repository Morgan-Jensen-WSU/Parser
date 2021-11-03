using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace parser
{
    class TableMaker
    {

        private const string FILE_PATH = "input/invalid_simple.txt";

        private static int ParseIndex { get; set; }
        private static string PrevWord { get; set; }

        private List<char> Input = new List<char>();
        private static Dictionary<int, List<string>> Productions = new Dictionary<int, List<string>>();
        private static Dictionary<string, List<string>> First = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> Follow = new Dictionary<string, List<string>>();
        private static Dictionary<string, Dictionary<string, List<string>>> FirstPlus = new Dictionary<string, Dictionary<string, List<string>>>();

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
            "^",
            "/",
            "@",
            "eof"
        };

        private static List<string> SampleTerminals = new List<string>
        {
            "num",
            "name",
            "(",
            ")",
            "+",
            "-",
            "*",
            "/",
            "eof"
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

        private static List<string> SampleNonTerminals = new List<string>
        {
            "Goal",
            "Expr",
            "Expr'",
            "Term",
            "Term'",
            "Factor"
        };

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
            // PrintTable();
            // PrintFirst();
            // PrintFollow();

            ParseIndex = 0;
            
            while (ParseIndex < Input.Count)
            { 
                if (Input[ParseIndex] == '\n')
                {
                    ParseIndex++;
                    continue;
                }

                if (Parse())
                {
                    Console.WriteLine("Valid");
                    PrevWord = null;
                }
                else
                {
                    Console.WriteLine("Invalid");
                    PrevWord = null;
                }
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
                else if (Terminals.Contains(focus.ToString()) || focus == "eof")   // focus is terminal
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

                    if (Table[focus][word] != -1)
                    {
                        parseStack.Pop();

                        var production = Productions[Table[focus][word]];

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

        private void FillSampleProduction()
        {
            Productions[0] = new List<string> 
            { 
                "Goal",
                "Expr" 
            };

            Productions[1] = new List<string> 
            { 
                "Expr",
                "Term",
                "Expr'" 
            };

            Productions[2] = new List<string> 
            { 
                "Expr'",
                "+",
                "Term",
                "Expr'" 
            };
            
            Productions[3] = new List<string> 
            { 
                "Expr'",
                "-",
                "Term",
                "Expr'" 
            };

            Productions[4] = new List<string> 
            { 
                "Expr'",
                "@" 
            };

            Productions[5] = new List<string> 
            { 
                "Term",
                "Factor",
                "Term'" 
            };

            Productions[6] = new List<string> 
            { 
                "Term'",
                "*",
                "Factor",
                "Term'" 
            };

            Productions[7] = new List<string> 
            { 
                "Term'",
                "/",
                "Factor",
                "Term'"
            };

            Productions[8] = new List<string> 
            { 
                "Term'",
                "@" 
            };

            Productions[9] = new List<string> 
            { 
                "Factor",
                "(",
                "Expr",
                ")"
            };

            Productions[10] = new List<string> 
            { 
                "Factor",
                "num"
            };

            Productions[11] = new List<string> 
            { 
                "Factor",
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

            // LTerm -> LFactor Term'
            Productions[2] = new List<string>
            {
                "LTerm",
                "LFactor",
                "Term'"
            };

            // RTerm -> RFactor Term'
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
                "@" 
            };

            // Term' -> * RTerm Term'
            Productions[7] = new List<string>
            {
                "Term'",
                "*",
                "RFactor",
                "Term'"
            };

            // Term' -> / RTerm Term'
            Productions[8] = new List<string>
            {
                "Term'",
                "/",
                "RFactor",
                "Term'"
            };

            // Term' -> e
            Productions[9] = new List<string> 
            { 
                "Term'",
                "@" 
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

            // Term' -> ^ RTerm Term'
            Productions[21] = new List<string>
            {
                "Term'",
                "^",
                "RFactor",
                "Term'"
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
            BuildFollow();
            BuildFirstPlus();
            GenerateTable();
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
                while (Input[ParseIndex] == ' ')
                {
                    ParseIndex++;
                }
                builder.Append(Input[ParseIndex]);
                ParseIndex++;
                
                if (Terminals.Contains(builder.ToString())) break;

                if (ParseIndex >= Input.Count) break;
            }
            while (!StoppingChar.Contains(Input[ParseIndex]));

            if (builder.ToString() == "-" && 
            !(PrevWord == "name" || PrevWord == "num" || PrevWord == ")" 
            || PrevWord == "spacenegname" || PrevWord == "spacenegnum"))
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

            int temp = 0;
            if (Int32.TryParse(builtString, out temp))
            {
                if (temp < 0) // neg number
                {
                    PrevWord = "spacenegnum";
                }
                else
                {
                    PrevWord = "num";
                }
            }
            else if (!Terminals.Contains(builtString))
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

            return PrevWord;
        }

        private void BuildFirst()
        {
            // for each t in (T U eof U e) First(t) <- t
            foreach (var t in Terminals)
            {
                First[t] = new List<string>(){ t };
            }
            First["@"] = new List<string> { "@" };
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
                        if (firstB != "@")
                        {
                            rhs.Add(firstB);
                        }
                    }

                    int i = 1;
                    while (First[b[i]].Contains("@") && i <= k - 1)
                    {
                        // rhs <- rhs U (FIRST(Bi + 1) - "@")
                        foreach(var firstB1 in First[b[i+1]])
                        {
                            if (firstB1 != "@")
                            {
                                rhs.Add(firstB1);
                            }
                        }
                        i += 1;
                    }

                    if (i == k && First[b[k]].Contains("@")) 
                    {
                        rhs.Add("@");
                    }

                    First[b[0]] = First[b[0]].Union(rhs).ToList();
                    
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

        private void BuildFollow()
        {
            // for each A in NT do;
                // FOLLOW(A) <- 0;
            // end;
            foreach (var nt in NonTerminals)
            {
                Follow[nt] = new List<string>();
            }

            // FOLLOW(S) <- {eof};
            Follow[Productions[0][0]].Add("eof");

            // while (FOLLOW sets are still changing) do;
            bool isChanging = true;
            while (isChanging)
            {
                isChanging = false;

                // for each p in P of the form A->B1B2...Bk do;
                foreach (var prod in Productions)
                {
                    List<string> b = prod.Value;
                    string a = b[0];
                    int k = b.Count - 1;

                    // TRAILER <- FOLLOW(A)
                    List<string> trailer = Follow[a].ToList();
                    
                    // for i <- k down to 1 do;
                    for (int i = k; i >= 1; i--)
                    {
                        // if Bi in NT then begin;
                        if (NonTerminals.Contains(b[i]))
                        {
                            List<string> checker = Follow[b[i]].ToList();
                            
                            // FOLLOW(Bi) <- FOLLOW(Bi) U TRAILER
                            Follow[b[i]] = Follow[b[i]].Union(trailer).ToList();

                            if (checker.Count != Follow[b[i]].Count)
                            {
                                isChanging = true;
                            }

                            // if e in FIRST(Bi)
                            if (First[b[i]].Contains("@"))
                            {
                                // then TRAILER <- TRAILER U (FIRST(Bi) - e)
                                List<string> rhs = new List<string>();
                                foreach (var val in First[b[i]])
                                {
                                    if (val != "@")
                                    {
                                        rhs.Add(val);
                                    }
                                }

                                    trailer = trailer.Union(rhs).ToList();
                            }
                            // else TRAILOR <- FIRST(Bi)
                            else
                            {
                                trailer = First[b[i]];
                            }
                        }
                        // else TRAILER <- FIRST(Bi)
                        else 
                        {
                            trailer = First[b[i]];
                        }
                    }
                }
            }
        }

        private void BuildFirstPlus()
        {
            foreach (var nt in NonTerminals)
            {
                FirstPlus[nt] = new Dictionary<string, List<string>>();
            }

            foreach (var t in Terminals)
            {
                FirstPlus[t] = new Dictionary<string, List<string>>();
            }

            foreach (var p in Productions)
            {
                var a = p.Value[0];
                var b = p.Value[1];

                if (!First[b].Contains("@"))
                {
                    FirstPlus[a][b] = First[b];
                }
                else
                {
                    FirstPlus[a][b] = First[b].Union(Follow[a]).ToList();
                }
            }

        }
    
        private void GenerateTable()
        {
            foreach (var nt in NonTerminals)
            {
                Table[nt] = new Dictionary<string, int>();
                foreach (var t in Terminals)
                {
                    Table[nt][t] = -1;
                }
            }

            for (int i = 0; i < Productions.Count; i++)
            {
                var a = Productions[i][0];
                var b = Productions[i][1];

                foreach (var val in FirstPlus[a][b])
                {
                    if (Terminals.Contains(val))
                    {
                        Table[a][val] = i;
                    }
                }

                if (FirstPlus[a][b].Contains("eof"))
                {
                    Table[a]["eof"] = i;
                }
            }
        }
    
        private void PrintTable()
        {
            string top = "";
            foreach (var t in Terminals)
            {
                if (t == "spacenegname") top += t;
                else top += "\t\t" + t;
            }
            Console.WriteLine(top);

            foreach (var nt in NonTerminals)
            {
                string line = nt;

                if (line != "SpaceNegVal") line += "\t\t";
                else line += "\t";

                foreach (var t in Terminals)
                {
                    if (Table[nt][t] == -1) line += "- \t\t";
                    else line += Table[nt][t] + "\t\t";
                }

                Console.WriteLine(line);
            }
        }

        private void PrintFirst()
        {
            foreach (var val in First)
            {
                string line = val.Key + ":";
                
                foreach (var thing in val.Value)
                {
                    line += "\t" + thing;
                }

                Console.WriteLine(line);
            }
        }

        private void PrintFollow()
        {
            foreach (var val in Follow)
            {
                string line = val.Key + ":";
                
                foreach (var thing in val.Value)
                {
                    line += "\t" + thing;
                }

                Console.WriteLine(line);
            }
        }
    }
}