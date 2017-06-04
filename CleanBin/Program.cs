using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CleanBin
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = false;

            Console.WriteLine("......................................................................");
            Console.WriteLine("CLEANBIN: v1 by Sveroa");

            var cmd = new Arguments(args);

            if (cmd["DIR"] == null)
            {
                Console.WriteLine("Missing /DIR:xxx parameter");
                return;
            }

            if (cmd["VERBOSE"] != null)
            {
                bool tmp;
                var sVerbose = cmd["VERBOSE"].ToLower();
                if (bool.TryParse(sVerbose, out tmp) == true)
                {
                    debug = tmp;
                }
            }

            var cleaner = new ProjectCleaner(cmd["DIR"], verbose: debug);

            cleaner.ProjectFolders.Add("*.csproj", new string[] { "bin", "obj" });
            cleaner.ProjectFolders.Add("*.sqlproj", new string[] { "bin", "obj" });
            cleaner.ProjectFolders.Add("*.sln", new string[] { "bin", "obj", "packages" });

            cleaner.Verbose = true;

            cleaner.Execute();
        }
    }
}
