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
        _pathDB = @"C:\Users\antmi\Documents\Unity\LSystemEvo\systems.json";
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
        RuleSet rs = new RuleSet("Fractal", "F-F-F-F", "FG", 90f);
        rs.AddRule("F", "F-FF--F-F");
        rs.AddRule("G", "F-FF--F-F");
        //rs.AddTerminal("G", "F");
        rs.Validate();

        RuleSet rs2 = new RuleSet("Fractal", "F-F-F-F", "FG", 90f);
        rs2.AddRule("F", "F+F-G-F+F");
        rs2.AddRule("G", "F-FF--F-F");
        //rs2.AddTerminal("G", "F");
        rs2.Validate();

        RuleSet rs3 = new RuleSet("Fractal", "+G", "FGH", 90f);
        rs3.AddRule("F", "F");
        rs3.AddRule("G", "+H-GG-H+");
        //rs3.AddRule("F", "F-Ff[F-F]-F+F");
        rs3.AddRule("H", "-G+HH+G-");
        //rs3.AddTerminal("G", "F");
        //rs3.AddTerminal("H", "F");
        rs3.Validate();


        RuleSet rs4 = new RuleSet("Fractal", "F-G-G-F+F-G-G-F", "FGH", 90f);
        rs4.AddRule("F", "F-G+F+G-F");
        rs4.AddRule("G", "GG");
        rs4.Validate();


        RuleSet rs5 = new RuleSet("Fractal", "F--F", "FG", 90f);
        rs5.AddRule("F", "F-FFGF");
        rs5.AddRule("G", "F-FG+");
        //rs5.AddTerminal("G", "F");
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
        //rs6.AddTerminal("G", "F");
        //rs6.AddTerminal("H", "F");
        rs6.Validate();

        RuleSet rs7 = new RuleSet("Hilbert 3D", "H-F&G^F", "FGH", 90f);
        rs7.AddRule("F", "FGHH&&G-");
        rs7.AddRule("G", "G-F&^F-F+G+F+G");
        rs7.AddRule("H", "F^-F-F^H+F+GF^");
        //rs7.AddTerminal("G", "F");
        //rs7.AddTerminal("H", "F");
        rs7.Validate();



        RuleSet rs8 = new RuleSet("Hilbert 3D", "G-G^G^G", "FGH", 90f);
        rs8.AddRule("F", "+H-£GG£-H+");
        rs8.AddRule("G", "+F&GHG&F+");
        rs8.AddRule("H", "-G^&&^F-");
        //rs8.AddTerminal("G", "F");
        //rs8.AddTerminal("H", "F");
        rs8.Validate();


        RuleSet rs9 = new RuleSet("Fractal", "-G", "FGH", 90f);
        rs9.AddRule("F", "F");
        rs9.AddRule("G", "+H^GG^H+");
        rs9.AddRule("H", "-G&HH&G-");
        //rs9.AddTerminal("G", "F");
        //rs9.AddTerminal("H", "F");
        rs9.Validate();


        RuleSet rs10 = new RuleSet("Hilbert 3D", "G", "FGH", 90f);
        rs10.AddRule("F", "H-");
        rs10.AddRule("G", "G-F+H&^&F+GFG++G");
        rs10.AddRule("H", "&^HG+-&-F^++G^F^");
        //rs10.AddTerminal("G", "F");
        //rs10.AddTerminal("H", "F");
        rs10.Validate();


        RuleSet rs11 = new RuleSet("Hilbert 3D", "F-F&F&F+F", "FGH", 90f);
        rs11.AddRule("F", "+H+H");
        rs11.AddRule("G", "G+G");
        rs11.AddRule("H", "F-&");
        //rs11.AddTerminal("G", "F");
        //rs11.AddTerminal("H", "F");
        rs11.Validate();

        RuleSet rs12 = new RuleSet("Fractal", "F-F-F-F-G", "FG", 90f);
        rs12.AddRule("F", "F+&F-G-F&+F");
        rs12.AddRule("G", "F-^FF--F^-F");
        //rs12.AddTerminal("G", "F");
        rs12.Validate();

        RuleSet rs13 = new RuleSet("Fractal", "H|H^ff^H|H", "FGH", 90f);
        rs13.AddRule("F", "H");
        rs13.AddRule("G", "+H!-GG-!H+");
        rs13.AddRule("H", "-G+HFH+G-");
        //rs13.AddTerminal("G", "F");
        //rs13.AddTerminal("H", "F");
        rs13.Validate();

        RuleSet rs14 = new RuleSet("Fractal", "F-G-F-G", "FGHI", 90f);
        rs14.AddRule("F", "F+F-G-F+F");
        rs14.AddRule("G", "H^HH^^H^H");
        rs14.AddRule("I", "G&GG&&G&G");
        //rs2.AddTerminal("G", "F");
        rs14.Validate();


        RuleSet rs15 = new RuleSet("Fractal", "$I^I^I^I-I-I-I^I^I^I^I", "FGHI", 90f);
        rs15.AddRule("F", "F");
        rs15.AddRule("G", "H^HH^^H^H");
        rs15.AddRule("H", "H");
        rs15.AddRule("I", "G&GG&&G&G");
        //rs2.AddTerminal("G", "F");
        rs15.Validate();

        RuleSet rs16 = new RuleSet("Fractal", "F^H&G-I", "FGHI", 90f);
        rs16.AddRule("F", "G+F+G");
        rs16.AddRule("G", "F-G-F");
        rs16.AddRule("H", "I+H+I");
        rs16.AddRule("I", "H-I-H");
        //rs5.AddTerminal("G", "F");
        rs16.Validate();






        //RuleSet rs12 = new RuleSet("Fractal", "F-F-F-F-", "FG", 90f);
        //rs12.AddRule("F", "F-G+G-F");
        //rs12.AddRule("G", "F+G-G-G+F");
        ////rs2.AddTerminal("G", "F");
        //rs12.Validate();


        var DB = new LSystemDB();
        DB.AddSystem(rs);
        DB.AddSystem(rs2);
        DB.AddSystem(rs3); 
        DB.AddSystem(rs4);
        DB.AddSystem(rs5);
        DB.AddSystem(rs6);
        DB.AddSystem(rs7);
        //DB.AddSystem(rs8);
        //DB.AddSystem(rs9);
        DB.AddSystem(rs10);
        //DB.AddSystem(rs11);
        DB.AddSystem(rs12);
        DB.AddSystem(rs13);
        DB.AddSystem(rs14);
        DB.AddSystem(rs15);
        //DB.AddSystem(rs16);

        DB.WriteToFile();
    }
}
