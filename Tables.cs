using System.Collections.Generic;

namespace parser
{
    class TableMaker
    {

        private static Dictionary<int, List<string>> Productions = new Dictionary<int, List<string>>();
        private static Dictionary<string, List<string>> First = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> Follow = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> FirstPlus = new Dictionary<string, List<string>>();
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

        public TableMaker()
        {

        }

        public void GenerateFirst()
        {
            Dictionary<string, List<string>> tFirsts = new Dictionary<string, List<string>>();
            foreach (var val in Terminals)
            {
                tFirsts[val] = new List<string> { val };
            }

            foreach (var val in NonTerminals)
            {
                First[val] = new List<string>();
            }
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


    }
}