﻿using Newtonsoft.Json;
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
        RuleSet rs = new RuleSet("Fractal","F-F-F-F", "FG", 90f);
        rs.AddRule("F", "F-FF--F-F");
        rs.AddRule("G", "F-FF--F-F");
        rs.AddTerminal("G", "F");
        rs.Validate();

        RuleSet rs2 = new RuleSet("Fractal", "F-F-F-F", "FG", 90f);
        rs2.AddRule("F", "F+F-G-F+F");
        rs2.AddRule("G", "F-FF--F-F");
        rs2.AddTerminal("G", "F");
        rs2.Validate();

        RuleSet rs3 = new RuleSet("Fractal", "-G", "FGH", 90f);
        rs3.AddRule("F", "F");
        rs3.AddRule("G", "+H-GG-H+");
        //rs3.AddRule("F", "F-Ff[F-F]-F+F");
        rs3.AddRule("H", "-G+HH+G-");
        rs3.AddTerminal("G", "F");
        rs3.AddTerminal("H", "F");
        rs3.Validate();

        RuleSet rs4 = new RuleSet("Fractal", "F", "FG", 90f);
        rs4.AddRule("F", "F-+G£\"");
        //rs3.AddRule("F", "F-Ff[F-F]-F+F");
        //rs4.AddRule("G", "F-FF--F-F");
        rs4.AddTerminal("G", "F£");
        rs4.Validate();

        RuleSet rs5 = new RuleSet("Fractal", "F-F&F-F", "FG", 90f);
        rs5.AddRule("F", "F-FFGF");
        rs5.AddRule("G", "F-FG+");
        rs5.AddTerminal("G", "F");
        rs5.Validate();



        //RuleSet rs4 = new RuleSet("Tree", "G", "FG", 45f);
        //rs4.AddRule("F", "FF");
        //rs4.AddRule("G", "F+[[G]-G]-F[-FG]+G");
        //rs4.AddTerminal("G", "");
        //rs4.Validate();

        //RuleSet rs5 = new RuleSet("Tree", "G", "FG", 25f);
        //rs5.AddRule("F", "FF");
        //rs5.AddRule("G", "F+[[G]-G]-F[-FG]+G");
        //rs5.AddTerminal("G", "");
        //rs5.Validate();

        RuleSet rs6 = new RuleSet("Hilbert 3D", "G", "FGH", 90f);
        rs6.AddRule("F", "F");
        rs6.AddRule("G", "G-F+HFH+F-&F^-F+&&GFG+F+G");
        rs6.AddRule("H", "&F^HFG^F^^^-F-F^H+F+G^F^");
        //rs6.AddRule("I", "-FX-Y");
        //rs6.AddRule("J", "-FX-Y");
        rs6.AddTerminal("G", "F");
        rs6.AddTerminal("H", "F");
        //rs6.AddTerminal("I", "F");
        //rs6.AddTerminal("J", "F");
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
