using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Post_Processor_System
{
    public static class PostProcessor
    {
        
/*Test comment in the
         post processor */
        
        // Testing

        public static List<string> getContentBetweenLines(string fileName, int beginLine, int endLine)
        {
            List<string> contentList = new List<string>();

            Console.WriteLine("FileName: " + fileName + "Lines: " + beginLine + "->" + endLine);
            for (int i = beginLine; i < endLine; i++)
            {
                string content = File.ReadLines(fileName).Skip(i).Take(endLine - i).First();
                string replacedContent = replaceHTMLSpecialCharacters(content);
                contentList.Add(replacedContent);
            }

            return contentList;
        }

        static string replaceHTMLSpecialCharacters(string content)
        {
            string replacedContent = "";
            replacedContent = content.Replace("&", "&amp");
            replacedContent = replacedContent.Replace("<", "&lt");
            return replacedContent;
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}