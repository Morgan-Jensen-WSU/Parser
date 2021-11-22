using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace parser
{
    class TableMaker
    {

        private const string FILE_PATH = "input/test.txt";
        private const string DEBUG_OUTPUT_PATH = "output/debug.txt";

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
            "ish",
            "name",
            "negnum",
            "negname",
            "spacenegnum",
            "spacenegname",
            "procedure",
            "result",
            ",",
            "(",
            ")",
            "{",
            "}",
            "+",
            "-",
            "*",
            "^",
            "/",
            "=",
            "@",
            "eof"
        };

        private static List<string> NonTerminals = new List<string>
        {
            "Goal",
            "LineFull",
            "VarTypeAfter",
            "LineVarName",
            "LineVarNameRemaining",
            "ProcedureParams",
            "Params",
            "MoreParams",
            "VarType",
            "Expr",
            "LTermAddSub",
            "LTermMultDiv",
            "RTermAddSub",
            "RTermMultDiv",
            "AddSub'",
            "MultDiv'",
            "MultAndRightOp",
            "DivAndRightOp",
            "Power'",
            "PowerAndRightOp",
            "LTermPower",
            "RTermPower",
            "GTerm",
            "Parens",
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

        public TableMaker()
        {
            TakeInput();
            FillProduction();
            FillTable();
            PrintTable();
            PrintFirst();
            PrintFollow();

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

        private void FillProduction()
        {
            // Goal -> LineFull
            Productions[0] = new List<string>
            {
                "Goal",
                "LineFull"
            };

            // LineFull -> VarType VarTypeAfter
            Productions[1] = new List<string>
            {
                "LineFull",
                "VarType",
                "VarTypeAfter"
            };

            // LineFull -> LineVarName
            Productions[2] = new List<string>
            {
                "LineFull",
                "LineVarName"
            };

            // LineFull -> negnum Power' MultDiv' AddSub'
            Productions[3] = new List<string>
            {
                "LineFull",
                "negnum",
                "Power'",
                "MultDiv'",
                "AddSub'"
            };

            // LineFull -> Parens Power' MultDiv' AddSub'
            Productions[4] = new List<string>
            {
                "LineFull",
                "Parens",
                "Power'",
                "MultDiv'",
                "AddSub'"
            };

            // LineFull -> result GTerm
            Productions[5] = new List<string>
            {
                "LineFull",
                "result",
                "GTerm"
            };

            // LineFull -> }
            Productions[6] = new List<string>
            {
                "LineFull",
                "}"
            };

            // VarTypeAfter -> LineVarName
            Productions[7] = new List<string>
            {
                "VarTypeAfter",
                "LineVarName"
            };

            // VarTypeAfter -> procedure name ProcedureParams {
            Productions[8] = new List<string>
            {
                "VarTypeAfter",
                "procedure",
                "name",
                "ProcedureParams",
                "{"
            };

            // LineVarName -> name LineVarNameRemaining
            Productions[9] = new List<string>
            {
                "LineVarName",
                "name",
                "LineVarNameRemaining"
            };

            // LineVarNameRemaining -> = Expr
            Productions[10] = new List<string>
            {
                "LineVarNameRemaining",
                "=",
                "Expr"
            };

            // LineVarNameRemaining -> PowerAndRightOp MultDiv' AddSub'
            Productions[11] = new List<string>
            {
                "LineVarNameRemaining",
                "PowerAndRightOp",
                "MultDiv'",
                "AddSub'"
            };

            // LineVarNameRemaining -> MultAndRightOp AddSub'
            Productions[12] = new List<string>
            {
                "LineVarNameRemaining",
                "MultAndRightOp",
                "AddSub'"
            };

            // LineVarNameRemaining -> DivAndRightOp AddSub'
            Productions[13] = new List<string>
            {
                "LineVarNameRemaining",
                "DivAndRightOp",
                "AddSub'"
            };

            // LineVarNameRemaining -> AddSub'
            Productions[14] = new List<string>
            {
                "LineVarNameRemaining",
                "AddSub'"
            };

            // ProcedureParams -> ( Params ) 
            Productions[15] = new List<string>
            {
                "ProcedureParams",
                "(",
                "Params",
                ")"
            };

            // Params -> VarType name MoreParams
            Productions[16] = new List<string>
            {
                "Params",
                "VarType",
                "name",
                "MoreParams"
            };

            // Params -> empty
            Productions[17] = new List<string>
            {
                "Params",
                "@"
            };

            // MoreParams -> , VarType name MoreParams
            Productions[18] = new List<string>
            {
                "MoreParams",
                ",",
                "VarType",
                "name",
                "MoreParams"
            };

            // MoreParams -> empty
            Productions[19] = new List<string>
            {
                "MoreParams",
                "@"
            };

            // VarType -> num
            Productions[20] = new List<string>
            {
                "VarType",
                "num"
            };

            // VarType -> ish
            Productions[21] = new List<string>
            {
                "VarType",
                "ish"
            };
            
            // Expr -> LTermAddSub AddSub'
            Productions[22] = new List<string>
            {
                "Expr",
                "LTermAddSub",
                "AddSub'"
            };

            // LTermAddSub -> LTermMultDiv MultDiv'
            Productions[23] = new List<string>
            {
                "LTermAddSub",
                "LTermMultDiv",
                "MultDiv'"
            };

            // LTerMultDiv -> LTermPower Power'
            Productions[24] = new List<string>
            {
                "LTermMultDiv",
                "LTermPower",
                "Power'"
            };

            // RTermAddSub -> RTermMultDiv MultDiv'
            Productions[25] = new List<string>
            {
                "RTermAddSub",
                "RTermMultDiv",
                "MultDiv'"
            };

            // RTermMultDiv -> RTermPower Power'
            Productions[26] = new List<string>
            {
                "RTermMultDiv",
                "RTermPower",
                "Power'"
            };

            // AddSub' -> + RTermAddSub AddSub'
            Productions[27] = new List<string>
            {
                "AddSub'",
                "+",
                "RTermAddSub",
                "AddSub'"
            };

            // AddSub' -> - RTermAddSub AddSub'
            Productions[28] = new List<string>
            {
                "AddSub'",
                "-",
                "RTermAddSub",
                "AddSub'"
            };

            // AddSub' -> empty
            Productions[29] = new List<string>
            {
                "AddSub'",
                "@"
            };

            // MultDiv' -> MultAndRightOp
            Productions[30] = new List<string>
            {
                "MultDiv'",
                "MultAndRightOp"
            };

            // MultDiv' -> DivAndRightOp
            Productions[31] = new List<string>
            {
                "MultDiv'",
                "DivAndRightOp"
            };

            // MultDiv' -> empty
            Productions[32] = new List<string>
            {
                "MultDiv'",
                "@"
            };

            // MultAndRightOp -> * RTermMultDiv MultDiv'
            Productions[33] = new List<string>
            {
                "MultAndRightOp",
                "*",
                "RTermMultDiv",
                "MultDiv'"
            };

            // DivAndRightOp -> / RTermMultDiv MultDiv'
            Productions[34] = new List<string>
            {
                "DivAndRightOp",
                "/",
                "RTermMultDiv",
                "MultDiv'"
            };

            // Power' -> PowerAndRightOp
            Productions[35] = new List<string>
            {
                "Power'",
                "PowerAndRightOp"
            };

            // Power' -> empty
            Productions[36] = new List<string>
            {
                "Power'",
                "@"
            };

            // PowerAndRightOp -> ^ RTermPower Power'
            Productions[37] = new List<string>
            {
                "PowerAndRightOp",
                "^",
                "RTermPower",
                "Power'"
            };

            //  LTermPower -> GTerm
            Productions[38] = new List<string>
            {
                "LTermPower",
                "GTerm"
            };

            //  LTermPower -> negnum
            Productions[39] = new List<string>
            {
                "LTermPower",
                "negnum"
            };

            //  LTermPower -> negname
            Productions[40] = new List<string>
            {
                "LTermPower",
                "negname"
            };

            //  RTermPower -> GTerm
            Productions[41] = new List<string>
            {
                "RTermPower",
                "GTerm"
            };

            //  GTerm -> Parens
            Productions[42] = new List<string>
            {
                "GTerm",
                "Parens"
            };

            //  GTerm -> PosVal
            Productions[43] = new List<string>
            {
                "GTerm",
                "PosVal"
            };

            //  GTerm -> SpaceNegVal
            Productions[44] = new List<string>
            {
                "GTerm",
                "SpaceNegVal"
            };

            //  Parens -> ( Expr )
            Productions[45] = new List<string>
            {
                "Parens",
                "(",
                "Expr",
                ")"
            };

            //  PosVal -> num
            Productions[46] = new List<string>
            {
                "PosVal",
                "num"
            };

            //  PosVal -> name
            Productions[47] = new List<string>
            {
                "PosVal",
                "name"
            };

            //  SpaceNegVal -> spacenegnum
            Productions[48] = new List<string>
            {
                "SpaceNegVal",
                "spacenegnum"
            };

            //  SpaceNegVal -> spacenegname
            Productions[49] = new List<string>
            {
                "SpaceNegVal",
                "spacenegname"
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
            if (File.Exists(DEBUG_OUTPUT_PATH)) File.Delete(DEBUG_OUTPUT_PATH);

            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append: true);
                        
            file.WriteLine("****************************************************************");
            file.WriteLine("Table");
            file.WriteLine("****************************************************************");
            string top = "";
            foreach (var t in Terminals)
            {
                if (t == "num") 
                {
                    top += "".PadRight(25);
                    top += t.PadRight(15);
                }
                else top += t.PadRight(15);
            }
            file.WriteLine(top);

            foreach (var nt in NonTerminals)
            {
                string line = nt.PadRight(25);

                foreach (var t in Terminals)
                {
                    if (Table[nt][t] == -1) line += "-".PadRight(15);
                    else line += Table[nt][t].ToString().PadRight(15);
                }

                file.WriteLine(line);
            }
        }

        private void PrintFirst()
        {
            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append:true); 
            file.WriteLine();

            file.WriteLine("****************************************************************");
            file.WriteLine("First");
            file.WriteLine("****************************************************************");
            foreach (var val in First)
            {
                string line = (val.Key + ":").PadRight(25);
                
                foreach (var thing in val.Value)
                {
                    line += thing.PadRight(15);
                }

                file.WriteLine(line);
            }
        }

        private void PrintFollow()
        {
            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append:true); 
            file.WriteLine();
            
            file.WriteLine("****************************************************************");
            file.WriteLine("Follow");
            file.WriteLine("****************************************************************");
            foreach (var val in Follow)
            {
                string line = (val.Key + ":").PadRight(25);
                
                foreach (var thing in val.Value)
                {
                    line += thing.PadRight(15);
                }

                file.WriteLine(line);
            }
        }
    }
}