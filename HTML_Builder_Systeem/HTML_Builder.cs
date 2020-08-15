using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Post_Processor_System;

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
        public List<string> contentList;
        public string bodyContent;

        public FileStructure(string fileName, string type, string name, int bLine, int eLine, List<string> contentList)
        {
            this.fileName = fileName;
            this.type = type;
            this.name = name;
            this.beginLine = bLine;
            this.endLine = eLine;
            cataloged = false;
            this.contentList = contentList;
            bodyContent = "";

        }
    }

    public static class HTML_Builder
        {
            private static string fullPath = Directory.GetCurrentDirectory() + @"\HTML_Project\";
            private static string htmlPageContent;

            private static List<FileStructure> fileStructureList = new List<FileStructure>();
            private static List<string> fileNameList = new List<string>();
            private static List<string> htmlPageList = new List<string>();


            public static void buildFileStructure(string fileName, string type, string name, int bLine, int eLine, List<string> contentList)
            {
                FileStructure structure =
                    new FileStructure(separateFileNameForHTML(fileName), type, name, bLine, eLine, contentList);
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
                    foreach (string fileName in files)
                    {
                        string strippedName = separateFileNameForHTML(fileName);
                        fileNameList.Add(strippedName);
                    }

                    string finalName = fullPath + separateFileNameForHTML(file) + ".html";
                    htmlPageList.Add(finalName);
                }
            }

            public static string separateFileNameForHTML(string fileName)
            {
                return fileName.Substring(fileName.LastIndexOf(@"\") + 1);
            }

            static void topOfCollapsablePanel(string type, string title, int begin, int end, int iteration)
            {
                string tag = type + title.Replace(" ", "") + "_" + iteration;
                htmlPageContent += "<div class=\"container\" style=\"width:inherit\">\n";
                htmlPageContent += "<div class=\"panel-group\">\n";
                htmlPageContent += "<div class=\"panel panel-default\">\n";
                htmlPageContent += "<div class=\"panel-heading\">\n";
                htmlPageContent += "<h4 class=\"panel-title\">\n";
                htmlPageContent += "<a data-toggle=\"collapse\" href=\"#" + tag + "\">" + title;
                if (type.Equals("namespace"))
                {
                    htmlPageContent += ";</a>\n";
                }
                else if(type.Equals("class"))
                {
                    htmlPageContent += "{</a>\n";
                }
                else
                {
                    htmlPageContent += "</a>\n";
                }

                htmlPageContent += "</h4>\n";
                htmlPageContent += "</div>\n";
                htmlPageContent += "<div id=\"" + tag + "\" class=\"panel-collapse collapse\">\n";
            }

            static void buildCollapsablePanel(List<FileStructure> currentFileStructure)
            {
                int iteration = 0;
                
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
                        }

                        topOfCollapsablePanel(currentType.type, currentType.name, currentType.beginLine, currentType.endLine, iteration);
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                        buildCollapsablePanel(currentFileStructure.ToList());
                    }
                    else if (currentType.type == "function" && currentType.name != "if" && currentType.name != "for" &&
                             currentType.name != "while" && currentType.name != "foreach")
                    {
                        topOfCollapsablePanel(currentType.type, currentType.name, currentType.beginLine, currentType.endLine, iteration);
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                        currentType.bodyContent = generateInteralContent(currentType.contentList);
                        htmlPageContent += currentType.bodyContent;
                        bottomOfCollapsablePanel();

                    }
                    else
                    {
                        currentFileStructure.ElementAt(currentFileStructure.IndexOf(currentType)).cataloged = true;
                    }
                    iteration++;
                }
            }

            //TODO move the comment processing over to the post processor
            static string generateInteralContent(List<string> contentList)
            {
                string bodyContent = "";
                bodyContent += "<pre><p>";
                List<String> multiLineComment = new List<string>();
                Boolean multi = false;
                
                foreach(string content in contentList)
                {
                    if(content.Trim().StartsWith("//") && multi == false)
                    {
                        bodyContent += markUpSingleLineComment(content);
                    }else if (content.Trim().StartsWith("*/") && multi == true )
                    {
                        multiLineComment.Add(content);
                        bodyContent += markUpMultiLineComment(multiLineComment);
                        multiLineComment.Clear();
                        multi = false;
                    }else if (!content.Trim().StartsWith("//") && !content.Trim().StartsWith("/*") && multi == false)
                    {
                        bodyContent += content + "\n";
                    }else if ((content.Trim().StartsWith("/*") && multi == false) || (content.Trim().StartsWith("/*") || multi == true))
                    {
                        multiLineComment.Add(content);
                        multi = true;
                    }
                }

                bodyContent += "</p></pre>";
                return bodyContent;
            }

            static string markUpMultiLineComment(List<String> commentLines)
            {
                String formattedComment = "";
                if (commentLines[0].Trim().Contains("=INFO"))
                {
                    foreach (String comment in commentLines)
                    {
                        formattedComment += "<mark style=\"background-color: #03fc0f\">" + comment + "</mark><br>\n";
                    }
                }else if (commentLines[0].Trim().Contains("=WARNING"))
                {
                    foreach (String comment in commentLines)
                    {
                        formattedComment += "<mark style=\"background-color: #fc031c\">" + comment + "</mark><br>\n";
                    }
                }
                else
                {
                    foreach (String comment in commentLines)
                    {
                        formattedComment += comment + "\n";
                    }
                }

                return formattedComment;
            }

            static string markUpSingleLineComment(string comment)
            {
                //=INFO Testing Info
                //=WARNING Testing Warning
                if (comment.Trim().Contains("=INFO"))
                {
                    return "<mark style=\"background-color: #03fc0f\">" + comment + "</mark><br>";
                }else if (comment.Trim().Contains("=WARNING"))
                {
                    return "<mark style=\"background-color: #fc031c\">" + comment + "</mark><br>";
                }
                else
                {
                    return comment;
                }
            }

            static void bottomOfCollapsablePanel()
            {
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
                htmlPageContent += "</div>\n";
            }

            public static void buildDependencyContent(string file)
            {
                htmlPageContent += "<div class = \"fileList\" style=\"text-align:center\">\n";
                htmlPageContent += "<h3>Active Dependencies for current file: </3>\n";
                htmlPageContent += "<ul class=\"list-group row\">\n";

                List<string> dependencies = PostProcessor.returnListOfDependencies(file);
                if (!dependencies.Any())
                {
                    htmlPageContent += "<li class=\"list-group-item col-xs-6\"><a href=\"" + "" + "\">" +
                                       "No dependencies" + "</a>" + "<br></li>\n";                
                }
                else
                {
                    foreach (string depend in dependencies)
                    {
                        string strippedName = separateFileNameForHTML(depend);
                        htmlPageContent += "<li class=\"list-group-item col-xs-6\"><a href=\"" + "file:///" + fullPath + strippedName + ".html" + "\">" +
                                           strippedName + ".html" + "</a>" + "<br></li>\n";
                    }   
                }


                htmlPageContent += "</ul>\n";
                htmlPageContent += "</div>\n";
            }

            public static void buildPageContent(string file, List<string> files)
            {

                htmlPageContent += buildHTMLHeader();
                
                htmlPageContent += "<div class = \"fileList\" style=\"text-align:center\">\n";
                htmlPageContent += "<h2>Project File List: </h2>\n";
                htmlPageContent += "<ul class=\"list-group row\">\n";
                foreach (string fileName in files)
                {
                    string strippedName = separateFileNameForHTML(fileName);
                    fileNameList.Add(strippedName);
                    htmlPageContent += "<li class=\"list-group-item col-xs-6\"><a href=\"" + "file:///" + fullPath + strippedName + ".html" + "\">" +
                                       strippedName + ".html" + "</a>" + "<br></li>\n";
                }

                htmlPageContent += "</ul>\n";
                htmlPageContent += "</div>\n";

                buildDependencyContent(file);
                

                List<FileStructure> currentFileStructure = new List<FileStructure>();

                foreach (FileStructure structure in fileStructureList)
                {
                    if (structure.fileName == file)
                    {
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


        }
    }