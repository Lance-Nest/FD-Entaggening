

using System.IO;
using System.Xml.Linq;

class tag
{
    public static string saveDirectory = "..\\..\\..\\..\\output\\";

    string modName;
    string registryName;
    string itemID;
    string category;
    string ingredientType;
    string type;
    string specific;
    string extra;
    string extra2;
    string servingStyle;
    string cooked;
    bool containsMeat;
    bool containsVeg;
    bool containsAnimalProduct;
    bool containsGrain;
    bool containsFruit;
    string output;

    List<string> tags = new List<string>();

    public static tag parceTag(string tagIn)
    {
        string[] values = tagIn.Split(',');
        tag t = new tag();

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
        t.output = values[11];
        return t;
    }

    public void save()
    {
        Console.WriteLine("====================================================================================");
        Console.WriteLine(registryName);
        Console.WriteLine("====================================================================================");

        Console.WriteLine("Edited Files:");
        Directory.CreateDirectory(tag.saveDirectory);
        write(category, tag.saveDirectory);
        write(ingredientType, tag.saveDirectory + "\\ingredient");
        write(type, tag.saveDirectory + "\\" + category);
        write(specific, tag.saveDirectory + "\\" + category + "\\" + type);
        write(extra, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific);
        write(extra2, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + extra);
        write(servingStyle, type, tag.saveDirectory + "\\" + category + "\\" + servingStyle);
        write(servingStyle, specific, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + servingStyle);
        write(servingStyle, extra, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + servingStyle);
        write(servingStyle, extra2, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
        write(cooked, type, tag.saveDirectory + "\\" + category + "\\" + servingStyle);
        write(cooked, specific, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + servingStyle);
        write(cooked, extra, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + servingStyle);
        write(cooked, extra2, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
        write(cooked, servingStyle, tag.saveDirectory + "\\" + category + "\\" + type + "\\" + specific + "\\" + servingStyle + "\\" + extra + "\\" + servingStyle);

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
                        outputFile.Write("{\n\t\"values\": { \n\t\t[\n");
                    else
                        outputFile.Write(",\n");

                    outputFile.Write("\t\t\t\"" + registryName + "\"");
                    outputFile.Close();
                    Console.WriteLine("\t" + path);
                }

                this.tags.Add(directory.Substring(directory.IndexOf(tag.saveDirectory) + tag.saveDirectory.Length) + "\\" + name);
            }
        }
    }

}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting");
        Console.WriteLine("Do Tags? (y\\n)");

        if (Console.ReadLine() == "y")
        {
            List<tag> tags = File.ReadAllLines("..\\..\\..\\..\\tags.csv").Skip(9).Select(t => tag.parceTag(t)).ToList();

            DirectoryInfo dir = new DirectoryInfo(tag.saveDirectory);
            dir.Delete(true);

            foreach (tag t in tags)
            {
                t.save();
            }

            //finalize all json
            string[] files = Directory.GetFiles(tag.saveDirectory, "*.json", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                using (StreamWriter outputFile = new StreamWriter(file, true))
                {
                    outputFile.WriteLine("\n\t\t]");
                    outputFile.WriteLine("\t}");
                    outputFile.WriteLine('}');
                }
            }
        }

        Console.WriteLine("Do Recipes? (y\\n)");

        if (Console.ReadLine() == "y")
        {
            string[] directories = Directory.GetDirectories("..\\..\\..\\..\\input");

            foreach (string directory in directories)
            {
                Console.WriteLine(directory);

                foreach(string f in Directory.GetFiles(directory))
                {
                    Console.WriteLine(f);
                    if (f != directory + "\\data")
                    {
                        File.Delete(f);
                    }
                }

                foreach (string f in Directory.GetDirectories(directory))
                {
                    Console.WriteLine(f);
                    if (f != directory + "\\data")
                    {
                        Directory.Delete(f, true);
                    }
                }


                if (Directory.Exists(directory + "\\data"))
                {
                    foreach (string mod in Directory.GetDirectories(directory + "\\data"))
                    {
                        if (Directory.Exists(mod + "\\recipes"))
                        {
                            foreach (string f in Directory.GetDirectories(mod))
                            {
                                Console.WriteLine(f);
                                if (f != mod + "\\recipes")
                                {
                                    Directory.Delete(f, true);
                                }
                            }

                            Console.WriteLine("\t" + mod);
                            string[] recipes = Directory.GetFiles(mod + "\\recipes");
                        }
                        else
                        {
                            Directory.Delete(mod, true);
                        }
                    }
                }
                else
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}