using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

//allows user to save LSystems to a JSON database
public class LSystemDB
{
    string _pathDB;
    Dictionary<string, RuleSet> _DB;

    public LSystemDB()
    {
        _pathDB = @"C:\Users\antmi\Documents\Unity\TurtleGraphics\systems.json";
        _DB = new Dictionary<string, RuleSet>();
    }

    public void AddSystem(RuleSet rs)
    {
        _DB.Add(_DB.Count.ToString(), rs);
    }

    public void WriteToFile()
    {
        File.WriteAllText(_pathDB, JsonConvert.SerializeObject(_DB, Formatting.Indented));
    }

    public Dictionary<string, RuleSet> ReadFromFile()
    {
        string filetext = File.ReadAllText(_pathDB);
        return JsonConvert.DeserializeObject<Dictionary<string, RuleSet>>(filetext);
    }
}
public static class InitialiseDB
{
    public static void Initialise()
    {
        //create ruleset
        RuleSet rs = new RuleSet("Fractal","F-F-F-F", "F", 90f);
        rs.AddRule("F", "F-FF--F-F");
        rs.Validate();

        RuleSet rs2 = new RuleSet("Fractal", "F-F-F-F", "F", 90f);
        rs2.AddRule("F", "F+F-F-F+F");
        rs2.Validate();

        RuleSet rs3 = new RuleSet("Fractal", "F-F-F-F", "F", 90f);
        rs3.AddRule("F", "F-Ff[F-F]-F+F");
        rs3.Validate();

        RuleSet rs4 = new RuleSet("Tree", "G", "FG", 45f);
        rs4.AddRule("F", "FF");
        rs4.AddRule("G", "F+[[G]-G]-F[-FG]+G");
        rs4.AddTerminal("G", "");
        rs4.Validate();

        RuleSet rs5 = new RuleSet("Tree", "G", "FG", 25f);
        rs5.AddRule("F", "FF");
        rs5.AddRule("G", "F+[[G]-G]-F[-FG]+G");
        rs5.AddTerminal("G", "");
        rs5.Validate();

        RuleSet rs6 = new RuleSet("Fractal", "FX", "FXY", 90f);
        rs6.AddRule("X", "X+YF+");
        rs6.AddRule("Y", "-FX-Y");
        rs6.AddTerminal("X", "");
        rs6.AddTerminal("Y", "");
        rs6.Validate();

        var DB = new LSystemDB();
        DB.AddSystem(rs);
        DB.AddSystem(rs2);
        DB.AddSystem(rs3); 
        DB.AddSystem(rs4);
        DB.AddSystem(rs5);
        DB.AddSystem(rs6);

        DB.WriteToFile();
    }
}
