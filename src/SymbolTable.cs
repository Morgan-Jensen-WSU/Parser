using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Compiler
{
    public class SymbolTable
    {
        private const string ASM_OUTPUT_PATH = "output/out.asm";

        private const string AUTO_INCLUDES_DATA = "\tfmtstr:\tdb \"%s\", 10, 0\n" +
                                                  "\tfmtuint:\tdb \"%d\", 10, 0\n" +
                                                  "\tfmtuintin:\tdb \"%d\", 0\n" +
                                                  "\tfmtfloatin:\tdb \"%f\", 0\n";   

        private const string END_ASM =  "\t; end program\n" +
                                        "\tmov\trax, 60\n" +
                                        "\txor\trdi, rdi\n" + 
                                        "\tsyscall\n";

        public static List<Variable> STable { get; set; }
        public static List<Variable> ExecTable { get; set; }

        public SymbolTable()
        {
            STable = new List<Variable>();
            ExecTable = new List<Variable>();
        }

        public static Variable Lookup(string name)
        {
            if (IsVarDefined(name))
            {
                return STable.FirstOrDefault(v => v.Name == name);
            }
            ErrMod.ThrowError($"Variable {name} does not exist in this context.");
            return null;
        }

        public static bool IsVarDefined(string name)
        {
            foreach (var s in STable)
            {
                if (s.Name == name) return true;
            }

            return false;
        }

        public void Insert(Variable.VarType type, string name, IR representation)
        {
            STable.Add(new Variable(type, name, representation));
        }

        public void Insert(Variable var)
        {
            STable.Add(var);
        }

        public void Update(string name, IR rep)
        {
            var v = STable.FirstOrDefault(v => v.Name == name);
            v.Rep = rep;
        }

        public void TranslateToASM()
        {
            foreach (Variable symbol in STable)
            {
                Translate(symbol);
                Output.DebugASM(symbol.Out.ASM);
            }
            PrintASM();
        }

        private void Translate(Variable var)
        {
            if (var.Rep.HasAnswer)
            {
                var.Section = Variable.ASMSection.data;
            }
            else
            {
                var.Section = Variable.ASMSection.bss;
            }

            var.Out = new Output(var);
        }

         public static void PrintASM()
        {
            if (File.Exists("output/out.asm")) File.Delete("output/out.asm");

            using StreamWriter file = new(ASM_OUTPUT_PATH, append:true);

            PrintData(file);
            PrintBSS(file);
            PrintText(file);
        }

        private static void PrintData(StreamWriter file)
        {
            file.WriteLine(";-------------------");
            file.WriteLine("; initialized data");
            file.WriteLine(";-------------------");

            file.WriteLine("\t\tsection .data");

            file.WriteLine(AUTO_INCLUDES_DATA);

            foreach (Variable var in STable.Where((s) => s.Section == Variable.ASMSection.data))
            {
                file.WriteLine($"\t{var.Out.ASM}");
            }
            
            file.WriteLine();
        }

        private static void PrintBSS(StreamWriter file)
        {
            file.WriteLine(";-------------------");
            file.WriteLine("; uninitialized data");
            file.WriteLine(";-------------------");

            file.WriteLine("\t\tsection .bss");
            foreach (Variable var in STable.Where((s) => s.Section == Variable.ASMSection.bss))
            {
                file.WriteLine($"\t{var.Out.ASM}");
            }

            file.WriteLine();
        }

        private static void PrintText(StreamWriter file)
        {
            file.WriteLine("\t\tsection .text");

            file.WriteLine(";-------------------");
            file.WriteLine("; imports ");
            file.WriteLine(";-------------------");
            file.WriteLine("\textern printf");
            file.WriteLine("\textern scanf");
            file.WriteLine("\tglobal main");

            file.WriteLine();

            file.WriteLine("main:");
            file.WriteLine("\tpush rbp\t; Push base pointer onto stack to save it\n");

            foreach (Variable var in ExecTable)
            {
                switch (var.Type)
                {
                    case Variable.VarType.printString:
                        PrintString(file, var.Name);
                        break;
                    case Variable.VarType.printNum:
                        PrintNum(file, var.Name);
                        break;
                    case Variable.VarType.printIsh:
                        PrintIsh(file, var.Name);
                        break;
                    case Variable.VarType.readNum:
                        ReadNum(file, var.Name);
                        break;
                    case Variable.VarType.readIsh:
                        ReadIsh(file, var.Name);
                        break;
                }
            }

            file.WriteLine(END_ASM);
        }

        private static void PrintString(StreamWriter file, string stringName)
        {
            file.WriteLine($"\t; write string to stdout");
            file.WriteLine($"\tmov\trsi, {stringName}");
            file.WriteLine($"\tmov\trdi, fmtstr");     
            file.WriteLine($"\tmov\trax, 0");           
            file.WriteLine($"\tcall printf"); 
            file.WriteLine();
        }

        private static void PrintNum(StreamWriter file, string numName)
        {
            file.WriteLine($"\t; write num to stdout");
            file.WriteLine($"\tmov\trsi, [{numName}]");
            file.WriteLine($"\tmov\trdi, fmtuint");
            file.WriteLine($"\tmov\trax, 0");
            file.WriteLine($"\tcall printf");
            file.WriteLine();
        }

        private static void PrintIsh(StreamWriter file, string ishName)
        {
            file.WriteLine($"\t; write ish to stdout");
            file.WriteLine($"\tlea\trdi, [fmtfloatin]");
            file.WriteLine($"\tmovss\txmm0, [{ishName}]");
            file.WriteLine($"\tcvtss2sd\txmm0, xmm0");
            file.WriteLine($"\tmov\trax, 1");
            file.WriteLine($"\tcall printf");
            file.WriteLine();
        }

        private static void ReadNum(StreamWriter file, string numName)
        {
            file.WriteLine($"\t; read in a number");
            file.WriteLine($"\tlea\trdi, [fmtuintin]");
            file.WriteLine($"\tlea\trsi, [{numName}]");
            file.WriteLine($"\tmov\trax, 0");
            file.WriteLine($"\tcall scanf");
            file.WriteLine();
        }

        private static void ReadIsh(StreamWriter file, string ishName)
        {
            file.WriteLine($"\t; read in a floating point");
            file.WriteLine($"\tlea\trdi, [fmtfloatin]");
            file.WriteLine($"\tlea\trsi, [{ishName}]");
            file.WriteLine($"\tcall scanf");
            file.WriteLine();
        }
    }
}