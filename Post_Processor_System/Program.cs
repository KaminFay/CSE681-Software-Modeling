using System;
using System.Collections.Generic;
using System.IO;

namespace Post_Processor_System
{
    static class Program
    {
        private static string htmlPageContent;

        static void generateFileList(List<string> files)
        {
            foreach (string file in files)
            {
                htmlPageContent += "<a href=\"" + file + "\">" + file + "</a>";
            }

            File.WriteAllText(@"C:\Users\Kamin\Documents\GitHub\CSE681-Software-Modeling\HTML_Project\testing.html", htmlPageContent);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}