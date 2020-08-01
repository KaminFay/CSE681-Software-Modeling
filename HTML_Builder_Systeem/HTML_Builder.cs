using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace HTML_Builder_System
{
    class FileStructure
    {
        public string fileName;
        public string type;
        public string name;
        public int beginLine;
        public int endLine;
        public bool cataloged;

        public FileStructure(string fileName, string type, string name, int bLine, int eLine)
        {
            this.fileName = fileName;
            this.type = type;
            this.name = name;
            this.beginLine = bLine;
            this.endLine = eLine;
            cataloged = false;
        }
    }

    public static class HTML_Builder
        {
            private static object locker = new Object();
            private static string fullPath = @"C:\Users\Kamin\Documents\GitHub\CSE681-Software-Modeling\HTML_Project\";
            private static string htmlPageContent;

            private static List<FileStructure> fileStructureList = new List<FileStructure>();
            private static List<string> fileNameList = new List<string>();
            private static List<string> htmlPageList = new List<string>();


            public static void buildFileStructure(string fileName, string type, string name, int bLine, int eLine)
            {
                FileStructure structure =
                    new FileStructure(separateFileNameForHTML(fileName), type, name, bLine, eLine);
                fileStructureList.Add(structure);
            }

            static string buildHTMLHeader()
            {
                string htmlHeader = "<!DOCTYPE html>" + "\n";
                htmlHeader += "<html>" + "\n";
                htmlHeader += "<head>" + "\n";
                htmlHeader += "    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" + "\n";
                htmlHeader +=
                    "    <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css\">" +
                    "\n";
                htmlHeader +=
                    "    <script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js\"></script>" +
                    "\n";
                htmlHeader +=
                    "    <script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js\"></script>" +
                    "\n";
                htmlHeader += "</head>" + "\n";
                htmlHeader += "<body>" + "\n";
                return htmlHeader;
            }

            public static void generateFileList(List<string> files)
            {
                foreach (string file in files)
                {
                    //htmlPageContent += buildHTMLHeader();

                    htmlPageContent += file + "<br>";
                    htmlPageContent += "File List: <br>";
                    foreach (string fileName in files)
                    {
                        string strippedName = separateFileNameForHTML(fileName);
                        fileNameList.Add(strippedName);
                        htmlPageContent += "<a href=\"" + "file:///" + fullPath + strippedName + ".html" + "\">" +
                                           strippedName + ".html" + "</a>" + "<br>";
                    }

                    string finalName = fullPath + separateFileNameForHTML(file) + ".html";
                    htmlPageList.Add(finalName);
                    //File.WriteAllText(finalName, htmlPageContent);
                    //htmlPageContent = String.Empty;
                }
            }

            public static string separateFileNameForHTML(string fileName)
            {
                return fileName.Substring(fileName.LastIndexOf(@"\") + 1);
            }

            static void build()
            {
                htmlPageContent += buildHTMLHeader();
            }

            static void topOfCollapsablePanel(string type, string title, int begin, int end)
            {
                htmlPageContent += "<div class=\"container\">\n";
                htmlPageContent += "<div class=\"panel-group\">\n";
                htmlPageContent += "<div class=\"panel panel-default\">\n";
                htmlPageContent += "<div class=\"panel-heading\">\n";
                htmlPageContent += "<h4 class=\"panel-title\">\n";
                htmlPageContent += "<a data-toggle=\"collapse\" href=\"#" + type + title.Replace(" ", "") + "\">" +
                                   type + ": " + title + " Lines:" + begin + "->" + end + "</a>\n";
                htmlPageContent += "</h4>\n";
                htmlPageContent += "<div id=\"" + type + title.Replace(" ", "") +
                                   "\" class=\"panel-collapse collapse\">\n";
                htmlPageContent += "<div class=\"panel-body\">\n";
            }

            static void buildCollapsablePanel(List<FileStructure> currentFileStructure)
            {

                foreach (var currentType in currentFileStructure.ToList())
                {
                    if (currentType.cataloged == true)
                        return;

                    if (currentType.type == "namespace" || currentType.type == "class" ||
                        currentType.type == "interface")
                    {
                        if (currentType.type == "class" && currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType) - 1).type == "function")
                        {
                            htmlPageContent += "</div>\n";
                            htmlPageContent += "</div>\n";
                            htmlPageContent += "</div>\n";
                            htmlPageContent += "</div>\n";
                            htmlPageContent += "</div>\n";
                            htmlPageContent += "</div>\n";
                        }

                        topOfCollapsablePanel(currentType.type, currentType.name, currentType.beginLine,
                            currentType.endLine);
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                        buildCollapsablePanel(currentFileStructure.ToList());
                    }
                    else if (currentType.type == "function" && currentType.name != "if" && currentType.name != "for" &&
                             currentType.name != "while" && currentType.name != "foreach")
                    {
                        topOfCollapsablePanel(currentType.type, currentType.name, currentType.beginLine,
                            currentType.endLine);
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                        htmlPageContent += "Datatatatat\n";
                        bottomOfCollapsablePanel();

                    }
                    else
                    {
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                    }

                    //bottomOfCollapsablePanel();
                }
            }


            static void bottomOfCollapsablePanel()
            {
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
            }

            public static void buildPageContent(string file, List<string> files)
            {

                htmlPageContent += buildHTMLHeader();
                

                foreach (string fileName in files)
                {
                    string strippedName = separateFileNameForHTML(fileName);
                    fileNameList.Add(strippedName);
                    htmlPageContent += "<a href=\"" + "file:///" + fullPath + strippedName + ".html" + "\">" +
                                       strippedName + ".html" + "</a>" + "<br>";
                }
                

                Console.WriteLine(file);
                List<FileStructure> currentFileStructure = new List<FileStructure>();

                foreach (FileStructure structure in fileStructureList)
                {
                    if (structure.fileName == file)
                    {
                        Console.WriteLine(structure.type + "--" + structure.beginLine + "--" + structure.endLine);
                        currentFileStructure.Add(structure);
                    }
                }

                buildCollapsablePanel(currentFileStructure);
                string finalName = fullPath + separateFileNameForHTML(file) + ".html";
                htmlPageList.Add(finalName);

                File.WriteAllText(finalName, htmlPageContent);

                htmlPageContent = String.Empty;
                currentFileStructure.Clear();
            }

            static void Main(string[] args)
            {
                Console.WriteLine("Hello World!");
            }
        }
    }