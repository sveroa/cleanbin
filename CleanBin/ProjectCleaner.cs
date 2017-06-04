using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanBin
{
    public class ProjectCleaner
    {
        public Dictionary<string, string[]> ProjectFolders { get; set; }

        public bool Verbose { get; set; }
        public string RootDirectory { get; set; }

        public ProjectCleaner(string rootDir, bool verbose = false)
        {
            var rootdir = rootDir.ToLower();
            if (string.IsNullOrEmpty(rootDir) == true)
            {
                throw new FormatException("Root Directory can not be empty or null");
            }

            if (Directory.Exists(rootdir) == false)
            {
                throw new DirectoryNotFoundException("Unknow directory '" + rootdir + "'");
            }

            ProjectFolders = new Dictionary<string, string[]>();
            RootDirectory = rootDir;
            Verbose = verbose;
        }

        public Dictionary<string, string[]> GetFilesToDelete(string root)
        {
            string[] dirs = Directory.GetDirectories(root, "*.*", SearchOption.AllDirectories);
            var projDirs = new Dictionary<string, string[]>();
            foreach (string dir in dirs)
            {
                for (int i = 0; i < ProjectFolders.Count; i++)
                {
                    var ext = ProjectFolders.ElementAt(i);
                    if (Directory.GetFiles(dir, ext.Key).Length == 0)
                        continue;
                    if (Verbose == true)
                        Console.WriteLine(ext.Key + ": found in '" + dir + "'");
                    for (int j = 0; j < ext.Value.Length; j++)
                    {
                        string delDir = dir + "\\" + ext.Value[j];
                        if (Directory.Exists(delDir) == false)
                            continue;

                        string[] delFiles = Directory.GetFiles(delDir, "*.*", SearchOption.AllDirectories);
                        if (Verbose == true)
                            Console.WriteLine("Found " + delFiles.Length + " files in " + delDir);
                        projDirs.Add(delDir, delFiles);
                    }
                }
            }
            return projDirs;
        }

        protected void DeleteEmptyDirectories(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteEmptyDirectories(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                    if (Verbose == true)
                        Console.WriteLine("Deleting sub directory '" + directory + "'");
                }
            }
        }

        public void Execute()
        {
            long totSize = 0;
            long totCount = 0;
            long totDelErrCount = 0;
            long totDelOKCount = 0;

            var deleteFiles = GetFilesToDelete(RootDirectory);
            for (int i=0; i < deleteFiles.Count; i++)
            {
                var ext = deleteFiles.ElementAt(i);
                if (Verbose == true)
                    Console.WriteLine("Cleaning " + ext.Key);

                FileInfo file;
                for (int fileIdx=0; fileIdx < ext.Value.Length; fileIdx++)
                {
                    string filename = "";

                    try
                    {
                        file = new FileInfo(ext.Value[fileIdx]);
                        filename = file.FullName;

                        totCount++;
                        totSize += file.Length;

                        file.Delete();
                        if (Verbose == true)
                            Console.WriteLine("Deleted OK '" + file.FullName + "'");
                        totDelOKCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR deleting '" + filename + "'"+ ex.Message);
                        totDelErrCount++;
                    }
                }

                try
                {
                    DeleteEmptyDirectories(ext.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn' delete '" + ext.Key + "'");
                }
            }

            totSize = ((totSize / 1024) / 1024);  // MB

            Console.WriteLine("\n......................................................................");
            Console.WriteLine("Number of files = " + totCount.ToString("#,#0"));
            Console.WriteLine("Space occupied  = " + totSize.ToString("#,#0") + " MB");
            Console.WriteLine("Deleted OK      = " + totDelOKCount.ToString("#,#0"));
            Console.WriteLine("Deleted Error   = " + totDelErrCount.ToString("#,#0"));
            Console.WriteLine("......................................................................");
        }
    }
}
