using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
    public class SymbolTable
    {

        public static List<Variable> STable { get; set; }

        public SymbolTable()
        {
            STable = new List<Variable>();
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

        public void Update(string name, IR rep)
        {
            var v = STable.FirstOrDefault(v => v.Name == name);
            v.Rep = rep;
        }
    }
}