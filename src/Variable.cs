
namespace Compiler
{
    public class Variable
    {
        public enum VarType
        {
            num,
            ish,
            err
        }

        public VarType Type { get; private set; }
        public string Name { get; private set; }
        public IR Rep { get; set; }

        public Variable(VarType type, string name, IR rep)
        {
            Type = type;
            Name = name;
            Rep = rep;
        }

        public static VarType DecideVarType(string type)
        {
            if (type == "num") return VarType.num;
            else if (type == "ish") return VarType.ish;
            else return VarType.err;
        }
    }
}