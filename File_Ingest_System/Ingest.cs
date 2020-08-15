using System;
using System.Collections.Generic;
using System.IO;

namespace File_Ingest_System
{
    public static class Ingest
    {
        private static string _projectDirectory;
        private static List<string> _CSharpFiles;

        // Using the passed in reference this will set the current directory of the project.
        private static void SetProjectDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                _projectDirectory = dir;
            }
            else
            {
                throw new InvalidDataException("The directory you passed in does not exist, please try again");
            }
        }

        //Recursively move through the file structure to locate any and all c# files. Ignoring obj compiled and Assembly files.
        private static List<string> SeparateCSharpFiles()
        {
            string[] filesInDirectory = Directory.GetFiles(_projectDirectory);
            foreach (string fileName in filesInDirectory)
            {
                if (fileName.EndsWith(".cs") && !fileName.Contains("\\obj\\") && !fileName.Contains("AssemblyInfo"))
                {
                    Console.WriteLine(fileName);
                    _CSharpFiles.Add(fileName);
                }
            }

            string[] subDirectories = Directory.GetDirectories(_projectDirectory);
            foreach (string subDirectory in subDirectories)
            {
                _projectDirectory = subDirectory;
                SeparateCSharpFiles();
            }

            return _CSharpFiles;
        }

        //Overarching generation function, this is called externally to pull all the data required. And handles 
        //invalid input.
        public static List<string> GenerateFileStructure(string[] args)
        {
            _CSharpFiles = new List<string>();
            try
            {
                SetProjectDirectory(args[0]);
            }catch (Exception e)
            {
                Console.WriteLine("Not enough arguments please try again + " + e.Message);
                Environment.Exit(1);
            }
            
            return SeparateCSharpFiles();
        }

    }
}