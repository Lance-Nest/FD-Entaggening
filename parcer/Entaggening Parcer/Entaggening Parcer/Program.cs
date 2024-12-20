﻿

using System.IO;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

class Tag
{
    public static string saveDirectory = "..\\..\\..\\..\\output\\data\\forge\\tags\\items\\";

    public string modName;
    public string registryName;
    public string itemID;
    public string category;
    public string ingredientType;
    public string type;
    public string specific;
    public string extra;
    public string extra2;
    public string servingStyle;
    public string cooked;

    List<string> tags = new List<string>();

    public static Tag parceTag(string tagIn)
    {
        string[] values = tagIn.Split(',');
        Tag t = new Tag();

        t.modName = values[0];
        t.registryName = values[1];
        t.itemID = values[2];
        t.category = values[3];
        t.ingredientType = values[4];
        t.type = values[5];
        t.specific = values[6];
        t.extra = values[7];
        t.extra2 = values[8];
        t.servingStyle = values[9];
        t.cooked = values[10];
        return t;
    }

    public void save()
    {
        Console.WriteLine("====================================================================================");
        Console.WriteLine(registryName);
        Console.WriteLine("====================================================================================");

        Console.WriteLine("Edited Files:");
        Directory.CreateDirectory(Tag.saveDirectory);
        write(category, Tag.saveDirectory);
        write(ingredientType, Tag.saveDirectory);
        write(type, Tag.saveDirectory);
        write(specific, Tag.saveDirectory +  "\\" + type);
        write(extra, Tag.saveDirectory + "\\"  + type + "\\" + specific);
        write(extra2, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + extra);
        write(servingStyle, type, Tag.saveDirectory + "\\"  + servingStyle);
        write(servingStyle, specific, Tag.saveDirectory + "\\"  + type + "\\" + servingStyle);
        write(servingStyle, extra, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + servingStyle);
        write(servingStyle, extra2, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
        write(cooked, type, Tag.saveDirectory + "\\"  + servingStyle);
        write(cooked, specific, Tag.saveDirectory + "\\"  + type + "\\" + servingStyle);
        write(cooked, extra, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + servingStyle);
        write(cooked, extra2, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
        write(cooked, servingStyle, Tag.saveDirectory + "\\"  + type + "\\" + specific + "\\" + servingStyle + "\\" + extra + "\\" + servingStyle);

        Console.WriteLine();

        Console.WriteLine("Tags:");
        foreach (string s in tags)
            Console.WriteLine("\t"+s);

        Console.WriteLine("====================================================================================");
        Console.WriteLine();
    }

    private void write(string name, string directory)
    {
        write(name, name, directory);
    }

    private void write(string name, string test, string directory)
    {
        if (name != null && name != "")
        {
            string path = Path.Combine(directory, name + ".json");
            if (test != null && test != "")
            {
                Directory.CreateDirectory(directory);
                bool exists = false;
                if (File.Exists(path))
                    exists = true;

                using (StreamWriter outputFile = new StreamWriter(path, true))
                {
                    if (!exists)
                        outputFile.Write("{\n\t\"values\":  \n\t\t[\n");
                    else
                        outputFile.Write(",\n");

                    outputFile.Write("\t\t\t\"" + registryName + "\"");
                    outputFile.Close();
                    Console.WriteLine("\t" + path);
                }

                this.tags.Add(directory.Substring(directory.IndexOf(Tag.saveDirectory) + Tag.saveDirectory.Length) + "\\" + name);
            }
        }
    }

    public string GetFullTag()
    {
        string s = "\"tag\": " + "\"forge:"+ type;
        if (specific != null && specific != "")
            s += "/" + specific;
        if (extra != null && extra != "")
            s += "/" + extra;
        if (extra2 != null && extra2 != "")
            s += "/" + extra;
        if (servingStyle != null && servingStyle != "")
            s += "/" + servingStyle;
        if (cooked != null && cooked != "")
            s += "/" + cooked;

        return s + "\"";
    }
}

//class Recipe
//{
//    public string location;
//    public Ingredient[]? ingredients { get; set; }
//    public Ingredient? ingredient { get; set; }
//    public Dictionary<string, Ingredient>? key { get; set; } = null;

//    public void SetLoc(string loc)
//    {
//        location = loc;
//    }

//    public void ToConsole()
//    {
//        Console.WriteLine(location);

//        if (ingredients != null)
//            foreach (Ingredient i in ingredients)
//            {
//                Console.WriteLine(i.item);
//            }

//        Console.WriteLine(ingredient?.item);

//        if (key != null)
//            foreach (KeyValuePair<string, Ingredient> i in key)
//            {
//                if(i.Value != null)
//                    Console.WriteLine(i.Key + ":" + i.Value.item);
//            }

//    }

//}

class Ingredient
{
    public string? item { get; set; } = "";
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting");
        List<Tag> tags = File.ReadAllLines("..\\..\\..\\..\\tags.csv").Skip(9).Select(t => Tag.parceTag(t)).ToList();
        //List<Recipe> recipes = new List<Recipe>();

        DirectoryInfo dir = new DirectoryInfo("..\\..\\..\\..\\output\\");
        if (dir.Exists)
            dir.Delete(true);

        foreach (Tag t in tags)
        {
            t.save();
        }

        //finalize all json
        string[] files = Directory.GetFiles(Tag.saveDirectory, "*.json", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            using (StreamWriter outputFile = new StreamWriter(file, true))
            {
                outputFile.WriteLine("\n\t\t]");
                //outputFile.WriteLine("\t}");
                outputFile.WriteLine('}');

                outputFile.Close();
            }
        }
        Console.WriteLine("Recipes Start:");
        string directory = "..\\..\\..\\..\\input";

        //Delete anything not in data
        cleanupAndReplaceRecipes(directory, tags);

        Console.WriteLine("Finish");
    }

    private static void cleanupAndReplaceRecipes(string directory, List<Tag> tags)
    {
            Console.WriteLine(directory);

            foreach (string f in Directory.GetDirectories(directory))
            {
                Console.WriteLine(f);
                if (f != directory.ToString() + "\\data")
                {
                    Directory.Delete(f,true);
                }


                if (Directory.Exists(f + "\\recipes"))
                {

                    //Convert Items to Tags
                    Console.WriteLine("\t" + f);
                    string[] recipeFiles = Directory.GetFiles(f + "\\recipes");

                    foreach (string recipeFile in recipeFiles)
                    {
                        string jsonString = File.ReadAllText(recipeFile);
                        if (jsonString != null && jsonString != "")
                        {
                            replaceItems(jsonString, tags, recipeFile);
                        }
                    }
                }
                else
                {
                    Directory.Delete(f, true);
                }
            }


        
    }

    private static void replaceItems(string jsonString, List<Tag> tags, string fileLoc)
    {
        bool edited = false;
        if (jsonString.Contains("\"item\":"))
        {
            for (int i = 0; ; i += "\"item\":".Length)
            {
                i = jsonString.IndexOf("\"item\":", i);

                if (i == -1)
                    break;

                int eol = jsonString.IndexOf('\n', i) - i;
                string toReplace = jsonString.Substring(i + "\"item\":".Length, eol - "\"item\":".Length);

                if (toReplace != null && toReplace != "")
                {
                    toReplace = toReplace.Replace("\"", "");
                    toReplace = toReplace.Replace(" ", "");
                    Tag t = tags.Find(t => t.registryName == toReplace);
                    if (t != null)
                    {
                        jsonString = jsonString.Replace(toReplace, t.GetFullTag());
                        Console.WriteLine(toReplace + " -> " + t.GetFullTag());

                        edited = true;
                    }
                }
            }

            if (edited)
            {
                string output = fileLoc.Replace("input", "output");
                Directory.CreateDirectory(output.Substring(0, output.LastIndexOf("\\")));
                File.WriteAllText(output, jsonString);
            }
        }
    }
}