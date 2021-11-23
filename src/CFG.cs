using System.Collections.Generic;

namespace Compiler
{
    public class CFG
    {

        public Dictionary<int, List<string>> Productions { get; set; }

        public CFG()
        {
            FillProductions();
        }

        private void FillProductions()
        {
            Productions = new Dictionary<int, List<string>>();

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

            //  PosVal -> num_value
            Productions[46] = new List<string>
            {
                "PosVal",
                "num_value"
            };

            // PosVal -> ish_value
            Productions[47] = new List<string>
            {
                "PosVal",
                "ish_value"
            };

            //  PosVal -> name
            Productions[48] = new List<string>
            {
                "PosVal",
                "name"
            };

            //  SpaceNegVal -> spacenegnum_value
            Productions[49] = new List<string>
            {
                "SpaceNegVal",
                "spacenegnum_value"
            };

            //  SpaceNegVal -> spacenegish_value
            Productions[50] = new List<string>
            {
                "SpaceNegVal",
                "spacenegish_value"
            };

            // SpaceNegVal -> spacenegname
            Productions[51] = new List<string>
            {
                "SpaceNegVal",
                "spacenegname"
            };

            // LineVarName -> PosVal LineVarNameRemaining
            Productions[52] = new List<string>
            {
                "LineVarName",
                "PosVal",
                "LineVarNameRemaining"
            };

            // LineVarName -> SpaceNegVal LineVarNameRemaining
            Productions[53] = new List<string>
            {
                "LineVarName",
                "SpaceNegVal",
                "LineVarNameRemaining"
            };
        }
    }
}