using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;

namespace File_Ingest_System
{
    public static class Ingest
    {
        private static string _projectDirectory;
        private static List<string> _CSharpFiles;

        public static string GetProjectDirectory()
        {
            return _projectDirectory;
        }

        public static void SetProjectDirectory(string dir)
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

        public static List<string> SeparateCSharpFiles()
        {
            string[] filesInDirectory = Directory.GetFiles(_projectDirectory);
            foreach (string fileName in filesInDirectory)
            {
                if (fileName.EndsWith(".cs") && !fileName.Contains("\\obj\\") && !fileName.Contains("\\AssemblyInfo"))
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

        public static List<string> GenerateFileStructure(string[] args)
        {
            _CSharpFiles = new List<string>();
            try
            {
                SetProjectDirectory(args[0]);
            }catch (Exception e)
            {
                switch (e)
                {
                    case IndexOutOfRangeException _:
                        Console.WriteLine("Not enough arguments please try again");
                        break;
                    case InvalidDataException _:
                        Console.WriteLine(e.Message);
                        break;
                }

                System.Environment.Exit(1);
            }
            
            return SeparateCSharpFiles();
        }
        
        public static void Main(string[] args)
        {
            
        }
        
    }
}