using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler
{
    public class TableMaker
    {
        private const string DEBUG_OUTPUT_PATH = "output/debug.txt";

        public static List<string> Terminals = new List<string>
        {
            "num",
            "ish",
            "num_value",
            "ish_value",
            "name",
            "negnum",
            "negname",
            "spacenegnum_value",
            "spacenegish_value",
            "spacenegname",
            "procedure",
            "return",
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
        public static List<string> NonTerminals = new List<string>
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

        public Dictionary<string, Dictionary<string, int>> Table { get; set; }

        private Dictionary<string, List<string>> First { get; set; }
        private Dictionary<string, List<string>> Follow { get; set; }
        private Dictionary<string, Dictionary<string, List<string>>> FirstPlus { get; set; }


        public CFG Language { get; set; }
        public TableMaker()
        {
            Language = new CFG();

            First = BuildFirst();
            Follow = BuildFollow();
            FirstPlus = BuildFirstPlus();
            Table = GenerateTable();

            PrintDebug();
        }

        private Dictionary<string, List<string>> BuildFirst()
        {

            Dictionary<string, List<string>> first = new Dictionary<string, List<string>>();
            // for each t in (T U eof U e) first(t) <- t
            foreach (var t in Terminals)
            {
                first[t] = new List<string>() { t };
            }
            first["@"] = new List<string> { "@" };
            first["eof"] = new List<string> { "eof" };

            // for each nt in (NT) first(nt) <- empty set
            foreach (var nt in NonTerminals)
            {
                first[nt] = new List<string>();
            }

            bool isChanging = true;

            while (isChanging)
            {
                isChanging = false;

                foreach (var prod in Language.Productions)
                {
                    List<string> rhs = new List<string>();
                    List<string> b = prod.Value;
                    int k = b.Count - 1;

                    int checker = first[b[0]].Count;

                    foreach (var firstB in first[b[1]])
                    {
                        if (firstB != "@")
                        {
                            rhs.Add(firstB);
                        }
                    }

                    int i = 1;
                    while (first[b[i]].Contains("@") && i <= k - 1)
                    {
                        // rhs <- rhs U (first(Bi + 1) - "@")
                        foreach (var firstB1 in first[b[i + 1]])
                        {
                            if (firstB1 != "@")
                            {
                                rhs.Add(firstB1);
                            }
                        }
                        i += 1;
                    }

                    if (i == k && first[b[k]].Contains("@"))
                    {
                        rhs.Add("@");
                    }

                    first[b[0]] = first[b[0]].Union(rhs).ToList();

                    if (!isChanging)
                    {
                        if (checker != first[b[0]].Count)
                        {
                            isChanging = true;
                        }
                    }
                }
            }

            return first;
        }

        private Dictionary<string, List<string>> BuildFollow()
        {
            Dictionary<string, List<string>> follow = new Dictionary<string, List<string>>();
            // for each A in NT do;
            // FOLLOW(A) <- 0;
            // end;
            foreach (var nt in NonTerminals)
            {
                follow[nt] = new List<string>();
            }

            // FOLLOW(S) <- {eof};
            follow[Language.Productions[0][0]].Add("eof");

            // while (FOLLOW sets are still changing) do;
            bool isChanging = true;
            while (isChanging)
            {
                isChanging = false;

                // for each p in P of the form A->B1B2...Bk do;
                foreach (var prod in Language.Productions)
                {
                    List<string> b = prod.Value;
                    string a = b[0];
                    int k = b.Count - 1;

                    // TRAILER <- FOLLOW(A)
                    List<string> trailer = follow[a].ToList();

                    // for i <- k down to 1 do;
                    for (int i = k; i >= 1; i--)
                    {
                        // if Bi in NT then begin;
                        if (NonTerminals.Contains(b[i]))
                        {
                            List<string> checker = follow[b[i]].ToList();

                            // FOLLOW(Bi) <- FOLLOW(Bi) U TRAILER
                            follow[b[i]] = follow[b[i]].Union(trailer).ToList();

                            if (checker.Count != follow[b[i]].Count)
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

            return follow;
        }

        private Dictionary<string, Dictionary<string, List<string>>> BuildFirstPlus()
        {
            Dictionary<string, Dictionary<string, List<string>>> firstPlus = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (var nt in NonTerminals)
            {
                firstPlus[nt] = new Dictionary<string, List<string>>();
            }

            foreach (var t in Terminals)
            {
                firstPlus[t] = new Dictionary<string, List<string>>();
            }

            foreach (var p in Language.Productions)
            {
                var a = p.Value[0];
                var b = p.Value[1];

                if (!First[b].Contains("@"))
                {
                    firstPlus[a][b] = First[b];
                }
                else
                {
                    firstPlus[a][b] = First[b].Union(Follow[a]).ToList();
                }
            }

            return firstPlus;
        }

        private Dictionary<string, Dictionary<string, int>> GenerateTable()
        {
            Dictionary<string, Dictionary<string, int>> table = new Dictionary<string, Dictionary<string, int>>();

            foreach (var nt in NonTerminals)
            {
                table[nt] = new Dictionary<string, int>();
                foreach (var t in Terminals)
                {
                    table[nt][t] = -1;
                }
            }

            for (int i = 0; i < Language.Productions.Count; i++)
            {
                var a = Language.Productions[i][0];
                var b = Language.Productions[i][1];

                foreach (var val in FirstPlus[a][b])
                {
                    if (Terminals.Contains(val))
                    {
                        table[a][val] = i;
                    }
                }

                if (FirstPlus[a][b].Contains("eof"))
                {
                    table[a]["eof"] = i;
                }
            }

            return table;
        }

        private void PrintDebug()
        {
            PrintTable();
            PrintFirst();
            PrintFollow();
        }

        private void PrintFirst()
        {
            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append: true);
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
            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append: true);
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

        private void PrintTable()
        {
            if (File.Exists(DEBUG_OUTPUT_PATH)) File.Delete(DEBUG_OUTPUT_PATH);

            using StreamWriter file = new(DEBUG_OUTPUT_PATH, append: true);

            file.WriteLine("****************************************************************");
            file.WriteLine("Table");
            file.WriteLine("****************************************************************");
            string top = "";
            foreach (var t in TableMaker.Terminals)
            {
                if (t == "num")
                {
                    top += "".PadRight(25);
                    top += t.PadRight(15);
                }
                else top += t.PadRight(15);
            }
            file.WriteLine(top);

            foreach (var nt in TableMaker.NonTerminals)
            {
                string line = nt.PadRight(25);

                foreach (var t in TableMaker.Terminals)
                {
                    if (Table[nt][t] == -1) line += "-".PadRight(15);
                    else line += Table[nt][t].ToString().PadRight(15);
                }

                file.WriteLine(line);
            }
        }
    }
}