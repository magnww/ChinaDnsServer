using System.IO;

namespace ChinaDnsServer
{
    public class Util
    {
        public static string GetWorkingDirectory()
        {
            return Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }
    }
}
