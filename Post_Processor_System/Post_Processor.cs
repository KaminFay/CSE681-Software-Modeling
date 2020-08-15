using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Post_Processor_System
{
    public class fileFunctionList
    {
        public List<string> functions;
        public string fileName;

        public fileFunctionList(List<string> functions, string fileName)
        {
            this.functions = functions;
            this.fileName = fileName;
        }
    }

    public class objectsInFile
    {
        public string fileName = "";
        public List<string> objects = new List<string>();
        
        public objectsInFile(string fileName, string firstObject)
        {
            this.fileName = fileName;
            objects.Add(firstObject);
        }
    }

    public class dependencies
    {
        public string fileName = "";
        public List<string> dependsOn = new List<string>();
    }

    public static class PostProcessor
    {
        public static List<dependencies> fullDependencyList = new List<dependencies>();
        public static List<objectsInFile> objectLists = new List<objectsInFile>();
        public static List<fileFunctionList> fileList = new List<fileFunctionList>();
        
        public static void addObjectToFile(String objectName, String name)
        {
                if (!objectLists.Exists(x => x.fileName == name))
                {
                    objectLists.Add(new objectsInFile(name, objectName));
                }
                else
                {
                    objectsInFile obj = objectLists.ElementAt(objectLists.IndexOf(objectLists.Find(x => x.fileName == name)));
                    objectsInFile newObj = obj;
                    newObj.objects.Add(objectName);
                    objectLists[objectLists.IndexOf(obj)] = newObj;
                }
        }
        
        public static void addFileToFileList(List<string> functionNames, string fileName)
        {
            fileList.Add(new fileFunctionList(functionNames, fileName));
        }


        public static void buildDependencies(List<string> fileNames)
        {
            foreach(string file in fileNames)
            {
                dependencies depends = new dependencies();

                List<string> dependsList = new List<string>();
                string fileContent = File.ReadAllText(file);
                foreach (objectsInFile obj in objectLists)
                {
                    foreach (string o in obj.objects)
                    {
                        if (!obj.fileName.Equals(file))
                        {
                            if (fileContent.Contains("new " + o) || fileContent.Contains("." + o + "(") || fileContent.Contains(o + "."))
                            {
                                if (!dependsList.Contains(obj.fileName))
                                {
                                    dependsList.Add(obj.fileName);   
                                }
                            }   
                        }
                    }
                }

                depends.dependsOn = dependsList;
                depends.fileName = file;
                fullDependencyList.Add(depends);
            }
        }
        
        public static List<string> returnListOfDependencies(string fileName)
        {
            
            return fullDependencyList.Find(x => x.fileName.EndsWith(fileName)).dependsOn;
        }

        public static List<string> getContentBetweenLines(string fileName, int beginLine, int endLine)
        {
            List<string> contentList = new List<string>();

            for (int i = beginLine; i < endLine; i++)
            {
                //TODO check here for possible dependencies against list of namespaces / classes
                
                string content = File.ReadLines(fileName).Skip(i).Take(endLine - i).First();
                findAnyDepends(content);
                string replacedContent = replaceHTMLSpecialCharacters(content);
                contentList.Add(replacedContent);
            }

            return contentList;
        }

        public static List<string> findAnyDepends(String lineOfCode)
        {
            List<string> depends = new List<string>();



            return depends;
        }

        static string replaceHTMLSpecialCharacters(string content)
        {
            string replacedContent = "";
            replacedContent = content.Replace("&", "&amp");
            replacedContent = replacedContent.Replace("<", "&lt");
            return replacedContent;
        }
        
    }
}