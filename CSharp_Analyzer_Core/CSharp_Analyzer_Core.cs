using System;
using System.Collections.Generic;
using CodeAnalysis;
using File_Ingest_System;
using HTML_Builder_System;

namespace CSharp_Analyzer_Core
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> CSharpFiles = Ingest.GenerateFileStructure(args);
            
            foreach (string file in CSharpFiles)
            {
              Console.Write("\n  Processing file {0}\n", System.IO.Path.GetFileName(file));
            
              CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
              semi.displayNewLines = false;
              if (!semi.open(file as string))
              {
                Console.Write("\n  Can't open {0}\n\n", args[0]);
                return;
              }
            
              Console.Write("\n  Type and Function Analysis");
              Console.Write("\n ----------------------------");
            
              BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
              Parser parser = builder.build();
            
              try
              {
                while (semi.getSemi())
                  parser.parse(semi);
                Console.Write("\n  locations table contains:");
              }
              catch (Exception ex)
              {
                Console.Write("\n\n  {0}\n", ex.Message);
              }
              Repository rep = Repository.getInstance();
              List<Elem> table = rep.locations;
              Console.Write(
                "\n  {0,10}, {1,25}, {2,5}, {3,5}, {4,5}, {5,5}, {6,5}, {7,5}",
                "category", "name", "bLine", "eLine", "bScop", "eScop", "size", "cmplx"
              );
              Console.Write(
                "\n  {0,10}, {1,25}, {2,5}, {3,5}, {4,5}, {5,5}, {6,5}, {7,5}",
                "--------", "----", "-----", "-----", "-----", "-----", "----", "-----"
              );
              foreach (Elem e in table)
              {
                if (e.type == "class" || e.type == "struct" || e.type == "Comment")
                  Console.Write("\n");
                Console.Write(
                  "\n  {0,10}, {1,25}, {2,5}, {3,5}, {4,5}, {5,5}, {6,5}, {7,5}", 
                  e.type, e.name, e.beginLine, e.endLine, e.beginScopeCount, e.endScopeCount+1,
                  e.endLine-e.beginLine+1, e.endScopeCount-e.beginScopeCount+1
                );
                HTML_Builder.buildFileStructure(file, e.type, e.name, e.beginLine, e.endLine);
              }
            
              Console.Write("\n");
              semi.close();
            }

            HTML_Builder.generateFileList(CSharpFiles);
            foreach (string file in CSharpFiles)
            {
              HTML_Builder.buildPageContent(file.Substring(file.LastIndexOf("\\") +1), CSharpFiles);
            }
            Console.Write("\n\n");
            
        }
    }
}