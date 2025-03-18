

using System.IO;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

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
        if (category != "seed")
        {
            write(type, Tag.saveDirectory);
            write(specific, Tag.saveDirectory + "\\" + type);
            write(extra, Tag.saveDirectory + "\\" + type + "\\" + specific);
            write(extra2, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + extra);
            write(servingStyle, type, Tag.saveDirectory + "\\" + servingStyle);
            write(servingStyle, specific, Tag.saveDirectory + "\\" + type + "\\" + servingStyle);
            write(servingStyle, extra, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + servingStyle);
            write(servingStyle, extra2, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
            write(cooked, type, Tag.saveDirectory + "\\" + servingStyle);
            write(cooked, specific, Tag.saveDirectory + "\\" + type + "\\" + servingStyle);
            write(cooked, extra, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + servingStyle);
            write(cooked, extra2, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + extra + "\\" + servingStyle);
            write(cooked, servingStyle, Tag.saveDirectory + "\\" + type + "\\" + specific + "\\" + servingStyle + "\\" + extra + "\\" + servingStyle);
        }
        else
        {
            write(type, category + "\\" + Tag.saveDirectory);
        }
        

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

class Ingredient
{
    public string? item { get; set; } = "";
}

class Program
{
    public static int editCount = 0;
    public static int deleteCount = 0;
    static void Main(string[] args)
    {
        Console.WriteLine("Starting");
        List<Tag> tags = File.ReadAllLines("..\\..\\..\\..\\tags.csv").Skip(9).Select(t => Tag.parceTag(t)).ToList();
        //List<Recipe> recipes = new List<Recipe>();

        DirectoryInfo dir = new DirectoryInfo("..\\..\\..\\..\\output\\");
        if (dir.Exists)
        {
            Console.WriteLine("Output already exists! Overwrite? (y/n)");

            while (true)
            {
                string? n = Console.ReadLine();

                if (n == "y")
                {
                    dir.Delete(true);
                    Taggify(tags);
                    break;
                }
                if (n == "n")
                    break;
            }
        }
        else
        {
            Taggify(tags);
        }

        Console.WriteLine("Begin Manual Review? (Y/N)");

        while (true)
        {
            string? n = Console.ReadLine();

            if (n == "y")
            {
                ManualReview("..\\..\\..\\..\\output\\");
            }
            if (n == "n")
                return;
        }
    }

    private static void Taggify(List<Tag> tags)
    {
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
        string directory = "..\\..\\..\\..\\input\\";

        //Delete anything not in data
        cleanupAndReplaceRecipes(directory, tags);

        Console.WriteLine("Finish");
        Console.WriteLine("Tags Created: " + tags.Count);
        Console.WriteLine("Files Edited: " + editCount);
        Console.WriteLine("Files Unedited: " + deleteCount);
    }

    private static void ManualReview(string directory)
    {
        string[] recipeFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);

        int c = 0;
        foreach (string file in recipeFiles)
        {
            c++;
            Console.Clear();

            Console.WriteLine(c + "/" + recipeFiles.Length + "\t" + file);

            IEnumerable<string> s = File.ReadLines(file);
            Dictionary<int, string> indexedStrings = new Dictionary<int, string>();
            
            for(int i = 0; i < s.Count(); i++)
            {
                string s2 = s.ElementAt(i);
                if (s2.Contains("\"tag\":") || s2.Contains("\"item\":"))
                {
                    indexedStrings.Add(i, s2);
                }
            }

            for(int i = 0; i < indexedStrings.Count()-1; i++)
            {
                Console.WriteLine((i+1) + ": " + indexedStrings.ElementAt(i).Value);
            }

            Console.WriteLine("Select between 1 and " + (indexedStrings.Count() - 1) + " or 0 to continue.");

            string? input = Console.ReadLine();
            while (input != null && input != "0")
            {
                int i = 0;
                if (int.TryParse(input, out i))
                {
                    Console.Clear();
                    Console.WriteLine("Change the following to?");
                    Console.WriteLine(indexedStrings.ElementAt(i-1).Value);
                    Console.Write("\"tag\": ");
                    string changed = getChangedInput();

                    KeyValuePair<int, string> kvp = indexedStrings.ElementAt(i - 1);

                    indexedStrings.Remove(kvp.Key);
                    indexedStrings.Add(kvp.Key, changed);

                    //TODO HERE
                }
            }
        }
    }

    private static string getChangedInput()
    {
        string? change = "";

        bool flag = false;
        while (!flag)
        {
            change = Console.ReadLine();
            if (change == null)
            {
                Console.WriteLine("Input empty!");
                flag = false;
            }
            else if (!change.Contains(':'))
            {
                Console.WriteLine("Input must follow proper syntax! ie \"mod:item/subitem\"");
                flag = false;
            }
            else
                flag = true;
        }

        return "\"tag\": " + change;
    }

    private static void cleanupAndReplaceRecipes(string directory, List<Tag> tags)
    {
        Console.WriteLine(directory);

        string[] allFiles = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        string[] recipeFiles = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);

        var deleteThese = allFiles.Except(recipeFiles);
        foreach (string file in deleteThese)
        {
            File.Delete(file);
        }

        DeleteEmptyDirs(directory);

        foreach (string f in recipeFiles)
        {
            Console.WriteLine(f);
            string jsonString = File.ReadAllText(f);
            if (jsonString != null && jsonString != "")
            {
                replaceItems(jsonString, tags, f);
            }
        }


    }

    private static void replaceItems(string jsonString, List<Tag> tags, string fileLoc)
    {
        bool edited = false;
        //split into lines
        string[] lines = jsonString.Split("\n");

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("\"result\""))
            {
                i += 2;
            }
            else
            {
                if (lines[i].Contains("\"item\":"))
                {

                    string registryName = lines[i].Replace(" ", "");
                    registryName = registryName.Replace("\"item\":", "");
                    registryName = registryName.Replace("\"", "");

                    List<Tag> t = tags.FindAll(t => t.registryName == registryName);

                    if (t != null && t.Count() > 0)
                    {

                        Tag specific = t.Aggregate(t[0], (cur, next) => next.GetFullTag().Length > cur.GetFullTag().Length ? next : cur);

                        if (specific != null)
                        {
                            Console.WriteLine(lines[i] + " -> " + specific.GetFullTag());
                            lines[i] = specific.GetFullTag();


                            edited = true;
                        }
                    }

                }
            }
        }

        if (edited)
        {
            string output = fileLoc.Replace("input", "output");
            Directory.CreateDirectory(output.Substring(0, output.LastIndexOf("\\")));

            Program.editCount++;

            using (StreamWriter outputFile = new StreamWriter(output, false))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
                outputFile.Close();
            }

            using (StreamWriter outputFile = new StreamWriter("..\\..\\..\\..\\effectedFiles.csv", true))
            {
                outputFile.WriteLine(output + ",");
                outputFile.Close();
            }
        }
        else
        {
            Program.deleteCount++;
            Console.WriteLine(fileLoc + " Deleted!");
            File.Delete(fileLoc);
        }
    }

    static void DeleteEmptyDirs(string dir)
    {
        if (String.IsNullOrEmpty(dir))
            throw new ArgumentException(
                "Starting directory is a null reference or an empty string",
                "dir");

        try
        {
            foreach (var d in Directory.EnumerateDirectories(dir))
            {
                DeleteEmptyDirs(d);
            }

            var entries = Directory.EnumerateFileSystemEntries(dir);

            if (!entries.Any())
            {
                try
                {
                    Directory.Delete(dir);
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
            }
        }
        catch (UnauthorizedAccessException) { }
    }
}