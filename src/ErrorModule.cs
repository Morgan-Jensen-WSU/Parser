using System.IO;

namespace Compiler
{
    public class ErrMod
    {   
        private const string ERROR_OUT_PATH = "output/errlog.txt";
        
        public static void ResetErrMod()
        {
            if (File.Exists(ERROR_OUT_PATH)) File.Delete(ERROR_OUT_PATH);
        }

        public static void ThrowError(string message)
        {   
            using (StreamWriter writer = new(ERROR_OUT_PATH, append:true))
            {
                writer.WriteLine($"ERROR: {message}");
            }
        }   
    }
}