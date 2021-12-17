
namespace Compiler
{
    public class Variable
    {
        public enum VarType
        {
            num,
            ish,
            printNum,
            printString,
            printIsh,
            readNum,
            readIsh,
            err
        }

        public enum ASMSection
        {
            bss,
            data,
            text
        }

        public VarType Type { get; private set; }
        public string Name { get; private set; }
        public IR Rep { get; set; }
        public Output Out { get; set; }
        public ASMSection Section { get; set; }

        public Variable(VarType type, string name, IR rep)
        {
            Type = type;
            Name = name;
            Rep = rep;
        }

        public static VarType DecideVarType(string type)
        {
            switch (type)
            {
                case "num":
                    return VarType.num;
                case "ish":
                    return VarType.ish;
                case "printNum":
                    return VarType.printNum;
                case "printString":
                    return VarType.printString;
                case "printIsh":
                    return VarType.printIsh;
                case "readNum":
                    return VarType.readNum;
                case "readIsh":
                    return VarType.readIsh;
                default:
                    return VarType.err;
            }
        }
    }
}