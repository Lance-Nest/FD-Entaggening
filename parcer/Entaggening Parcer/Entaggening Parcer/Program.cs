

using System.IO;
using System.Xml.Linq;

class tag
{
    public static string saveDirectory = "../../../../output/";

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
        Console.WriteLine(itemID);
        Directory.CreateDirectory(tag.saveDirectory);
        write(category, tag.saveDirectory);
        write(ingredientType, tag.saveDirectory + "/ingredient");
        write(type, tag.saveDirectory);
        write(specific, tag.saveDirectory + "/" + type);
        write(extra, tag.saveDirectory + "/" + type + "/" + specific);
        write(extra2, tag.saveDirectory + "/" + type + "/" + specific + "/" + extra);
        write(servingStyle, type, tag.saveDirectory + "/" + servingStyle);
        write(servingStyle, specific, tag.saveDirectory + "/" + type + "/" + servingStyle);
        write(servingStyle, extra, tag.saveDirectory + "/" + type + "/" + specific + "/" + servingStyle);
        write(servingStyle, extra2, tag.saveDirectory + "/" + type + "/" + specific + "/" + extra + "/" + servingStyle);
        write(cooked, type, tag.saveDirectory + "/" + servingStyle);
        write(cooked, specific, tag.saveDirectory + "/" + type + "/" + servingStyle);
        write(cooked, extra, tag.saveDirectory + "/" + type + "/" + specific + "/" + servingStyle);
        write(cooked, extra2, tag.saveDirectory + "/" + type + "/" + specific + "/" + extra + "/" + servingStyle);
        write(cooked, servingStyle, tag.saveDirectory + "/" + type + "/" + specific + "/" + servingStyle + "/" + extra + "/" + servingStyle);
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
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting");

        List<tag> tags = File.ReadAllLines("../../../../tags.csv").Skip(9).Select(t => tag.parceTag(t)).ToList();

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
}