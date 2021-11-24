using System.Collections.Generic;

namespace Compiler
{
    public class SymbolTable
    {

        public Dictionary<string, IR> STable { get; set; }

        public SymbolTable()
        {
            STable = new Dictionary<string, IR>();
        }




    }
}