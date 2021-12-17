using System.IO;

namespace Compiler
{
    public class Output
    {
        private const string ASM_DEMO_OUTPUT_PATH = "output/asm.txt";
        public string ASM { get; set; }

        public Output(Variable var)
        {
            if (var.Section == Variable.ASMSection.data)
            {
                if (var.Type == Variable.VarType.printString)
                {
                    ASM = $"{var.Name}:\tdb\t\"{var.Rep.Answer}\", 0\t; a const string";
                }
                else if (var.Type == Variable.VarType.num || var.Type == Variable.VarType.ish)
                {
                    ASM = $"{var.Name}:\tdd\t{var.Rep.Answer}\t; a(n) {var.Type}";
                }
            }
            else if (var.Section == Variable.ASMSection.bss)
            {
                if (var.Type == Variable.VarType.num)
                {
                    ASM = $"{var.Name}:\tresd\t1\t; an int";
                }
                else if (var.Type == Variable.VarType.ish)
                {
                    ASM = $"{var.Name}:\tresd\t1\t; a float";
                }
            }
        }

        public static void DebugASM(string asm)
        {
            using StreamWriter file = new(ASM_DEMO_OUTPUT_PATH, append:true);

            file.WriteLine(asm);
        }

    }
}