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

            // LineFull -> ExprWithoutName
            Productions[3] = new List<string>
            {
                "LineFull",
                "ExprWithoutName"
            };

            // LineFull -> return GTerm
            Productions[4] = new List<string>
            {
                "LineFull",
                "return",
                "GTerm"
            };

            // LineFull -> if ( Condition ) {
            Productions[5] = new List<string>
            {
                "LineFull",
                "if",
                "(",
                "Condition",
                ")",
                "{"
            };

            // LineFull -> else {
            Productions[6] = new List<string>
            {
                "LineFull",
                "else",
                "{"
            };

            // LineFull -> }
            Productions[7] = new List<string>
            {
                "LineFull",
                "}"
            };

            // LineFull -> printNum name
            Productions[8] = new List<string>
            {
                "LineFull", 
                "printNum",
                "name"
            };

            // LineFull -> printIsh name
            Productions[9] = new List<string>
            {
                "LineFull",
                "printIsh",
                "name"
            };

            // LineFull readNum name
            Productions[10] = new List<string>
            {
                "LineFull",
                "readNum",
                "name"
            };

            // LineFull readIsh name
            Productions[11] = new List<string>
            {
                "LineFull",
                "readIsh",
                "name"
            };

           // LineFull printString " string_value "
            Productions[12] = new List<string>
            {
                "LineFull",
                "printString",
                "\"",
                "string_value",
                "\""
            };

            // VarTypeAfter LineVarName
            Productions[13] = new List<string>
            {
                "VarTypeAfter",
                "LineVarName"
            };

            // VarTypeAfter -> procedure name ProcedureParams {
            Productions[14] = new List<string>
            {
                "VarTypeAfter",
                "procedure",
                "name",
                "ProcedureParams",
                "{"                
            };

            // LineVarName name LineVarNameRemaining
            Productions[15] = new List<string>
            {
                "LineVarName",
                "name",
                "LineVarNameRemaining"
            };

            // LineVarNameRemaining -> = Expr
            Productions[16] = new List<string>
            {
                "LineVarNameRemaining",
                "=",
                "Expr"
            };

            // LineVarNameRemaining -> PowerAndRightOp MultDiv' AddSub'
            Productions[17] = new List<string>
            {
                "LineVarNameRemaining",
                "PowerAndRightOp",
                "MultDiv'",
                "AddSub'"
            };

            // LineVarNameRemaining -> MultAndRightOp AddSub'
            Productions[18] = new List<string>
            {
                "LineVarNameRemaining",
                "MultAndRightOp",
                "AddSub'"
            };

            // LineVarNameRemaining -> DivAndRightOp AddSub'
            Productions[19] = new List<string>
            {
                "LineVarNameRemaining",
                "DivAndRightOp",
                "AddSub'"
            };

            // LineVarNameRemaining -> AddSub'
            Productions[20] = new List<string>
            {
                "LineVarNameRemaining",
                "AddSub'"
            };

            // ExprWithoutName -> num_value Power' MultDiv' AddSub'
            Productions[21] = new List<string>
            {
                "ExprWithoutName",
                "num_value",
                "Power'",
                "MultDiv'",
                "AddSub'"
            };
            
            // ExprWithoutName -> negnum_value Power' MultDiv' AddSub'
            Productions[22] = new List<string>
            {
                "ExprWithoutName",
                "negnum_value",
                "Power'",
                "MultDiv'",
                "AddSub'"
            };

            // ExprWithoutName -> Parens Power' MultDiv' AddSub'
            Productions[23] = new List<string>
            {
                "ExprWithoutName",
                "Parens",
                "Power'",
                "MultDiv'",
                "AddSub'"
            };

            // Condition -> Expr = = Expr
            Productions[24] = new List<string>
            {
                "Condition",
                "Expr",
                "=",
                "=",
                "Expr"
            };

            // Condition -> Expr ! = Expr
            Productions[25] = new List<string>
            {
                "Condition",
                "Expr",
                "!",
                "=",
                "Expr"
            };

            // ProcedureParams -> ( Params )
            Productions[26] = new List<string>
            {
                "ProcedureParams",
                "(",
                "Params",
                ")"
            };

            // Params -> VarType name MoreParams
            Productions[27] = new List<string>
            {
                "Params",
                "VarType",
                "name",
                "MoreParams"
            };

            // Params -> empty
            Productions[28] = new List<string>
            {
                "Params",
                "@"
            };

            // MoreParams -> , VarType name MoreParams
            Productions[29] = new List<string>
            {
                "MoreParams",
                ",",
                "VarType",
                "name",
                "MoreParams"
            };

            // MoreParams -> empty
            Productions[30] = new List<string>
            {
                "MoreParams",
                "@"
            };

            // VarType -> num
            Productions[31] = new List<string>
            {
               "VarType",
               "num"
            };

            // VarType -> ish
            Productions[32] = new List<string>
            {
                "VarType",
                "ish"
            };

            // Expr -> LTermAddSub  AddSub' 
            Productions[33] = new List<string>
            {
                "Expr",
                "LTermAddSub",
                "AddSub'"
            };

            // LTermAddSub -> LTermMultDiv MultDiv'
            Productions[34] = new List<string>
            {
                "LTermAddSub",
                "LTermMultDiv",
                "MultDiv'"
            };

            // LTermMultDiv -> LTermPower Power'
            Productions[35] = new List<string>
            {
                "LTermMultDiv",
                "LTermPower",
                "Power'"
            };

            // RTermAddSub -> RTermMultDiv MultDiv'
            Productions[36] = new List<string>
            {
                "RTermAddSub",
                "RTermMultDiv",
                "MultDiv'"
            };

            // RTermMultDiv -> RTermPower Power'
            Productions[37] = new List<string>
            {
                "RTermMultDiv",
                "RTermPower",
                "Power'"
            };

            // AddSub' -> + RTermAddSub AddSub'
            Productions[38] = new List<string>
            {
                "AddSub'",
                "+",
                "RTermAddSub",
                "AddSub'"
            };

            // AddSub' -> - RTermAddSub AddSub'
            Productions[39] = new List<string>
            {
                "AddSub'",
                "-",
                "RTermAddSub",
                "AddSub'"
            };

            // AddSub' -> empty
            Productions[40] = new List<string>
            {
               "AddSub'",
               "@"
            };

            // MultDiv' -> MultAndRightOp
            Productions[41] = new List<string>
            {
                "MultDiv'",
                "MultAndRightOp"
            };

            // MultDiv' -> DivAndRightOp
            Productions[42] = new List<string>
            {
                "MultDiv'",
                "DivAndRightOp"
            };

           // MultDiv' -> empty
            Productions[43] = new List<string>
            {
                "MultDiv'",
                "@"
            };

            // MultAndRightOp -> * RTermMultDiv MultDiv'
            Productions[44] = new List<string>
            {
                "MultAndRightOp",
                "*",
                "RTermMultDiv",
                "MultDiv'"
            };

            // DivAndRightOp -> / RTermMultDiv MultDiv'
            Productions[45] = new List<string>
            {
                "DivAndRightOp",
                "/",
                "RTermMultDiv",
                "MultDiv'"
            };

            // Power' -> PowerAndRightOp
            Productions[46] = new List<string>
            {
                "Power'",
                "PowerAndRightOp"
            };

            // Power' -> empty
            Productions[47] = new List<string>
            {
                "Power'",
                "@"
            };

            // PowerAndRightOp -> ^ RTermPower Power'
            Productions[48] = new List<string>
            {
                "PowerAndRightOp",
                "^",
                "RTermPower",
                "Power'"
            };

            // LTermPower -> GTerm
            Productions[49] = new List<string>
            {
                "LTermPower",
                "GTerm"
            };

            // LTermPower -> negnum_value
            Productions[50] = new List<string>
            {
                "LTermPower",
                "negnum_value"
            };

            // LTermPower -> negish_value
            Productions[51] = new List<string>
            {
                "LTermPower",
                "negish_value"
            };

            // LTermPower -> negname
            Productions[52] = new List<string>
            {
                "LTermPower",
                "negname"
            };

            // RTermPower -> GTerm
            Productions[53] = new List<string>
            {
                "RTermPower",
                "GTerm"
            };

            // GTerm -> NameOrProcedure
            Productions[54] = new List<string>
            {
                "GTerm",
                "NameOrProcedure"
            };
            
            // GTerm -> Parens
            Productions[55] = new List<string>
            {
                "GTerm",
                "Parens"
            };
            
            // GTerm -> num_value
            Productions[56] = new List<string>
            {
                "GTerm",
                "num_value"
            };
            
            // GTerm -> ish_value
            Productions[57] = new List<string>
            {
                "GTerm",
                "ish_value"
            };
            
            // GTerm -> SpaceNegVal
            Productions[58] = new List<string>
            {
                "GTerm",
                "SpaceNegVal"
            };
            
            // NameOrProcedure -> name Arguments
            Productions[59] = new List<string>
            {
                "NameOrProcedure",
                "name",
                "Arguments"
            };
            
            // Arguments -> ( Expr MoreArguments )
            Productions[60] = new List<string>
            {
                "Arguments",
                "(",
                "Expr",
                "MoreArguments",
                ")"
            };
            
            // Arguments -> empty
            Productions[61] = new List<string>
            {
                "Arguments",
                "@"
            };
            
            // MoreArguments -> , Expr MoreArguments
            Productions[62] = new List<string>
            {
                "MoreArguments",
                ",",
                "Expr",
                "MoreArguments"
            };
            
            // MoreArguments -> empty
            Productions[63] = new List<string>
            {
                "MoreArguments",
                "@"
            };
            
            // Parens -> ( Expr )
            Productions[64] = new List<string>
            {
                "Parens",
                "(",
                "Expr",
                ")"
            };
            
            // SpaceNegVal -> spacenegnum_value
            Productions[65] = new List<string>
            {
                "SpaceNegVal",
                "spacenegnum_value"
            };
            
            // SpaceNegVal -> spacenegish_value
            Productions[66] = new List<string>
            {
               "SpaceNegVal",
               "spacenegish_value"
            };

            // SpaceNegVal -> spacenegname
            Productions[67] = new List<string>
            {
                "SpaceNegVal",
                "spacenegname"
            };
        }
    }
}