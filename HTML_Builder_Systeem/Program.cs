using System;
using System.Collections.Generic;
using System.IO;

namespace HTML_Builder_System
{
    public static class HTML_Builder
    {
        private static string htmlPageContent;

        public static void generateFileList(List<string> files)
        {
            foreach (string file in files)
            {
                htmlPageContent += "<a href=\"" + file + "\">" + file + "</a>" + "<br>";
            }

            File.WriteAllText(@"C:\Users\Kamin\Documents\GitHub\CSE681-Software-Modeling\HTML_Project\testing.html", htmlPageContent);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}