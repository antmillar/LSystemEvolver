using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class LSystem
{
    private string _axiom;
    private string _generatedString;
    private int _numIterations;
    private RuleSet _ruleSet;
    public LSystem(string axiom, int numIterations, RuleSet rs)
    {
        _axiom = axiom;
        _numIterations = numIterations;
        _ruleSet = rs;
    }

    //maps the final generation with terminals
    private void ApplyTerminals()
    {

        foreach (string s in _ruleSet._terminals.Keys)
        {
            _generatedString = _generatedString.Replace(s, _ruleSet._terminals[s]);
        }
    }

    public string Generate()
    {

        if (_ruleSet._valid && _ruleSet._rules.Count != 0)
        {
            _generatedString = ReplaceRecursive(_axiom, _numIterations);
            ApplyTerminals();
            return _generatedString;
        }
        else
        {
            Debug.Log("ERROR : LSystem not generated, problem with setup, please check");
            return null;
        }
    }

    public void Information()
    {

        Debug.Log(string.Format("Category : {0}", _ruleSet._category));
        Debug.Log(string.Format("Axiom : {0}", _axiom));
        foreach (KeyValuePair<string, string> rule in _ruleSet._rules)
        {
            Debug.Log(string.Format("Rule : {0} > {1}", rule.Key, rule.Value));
        }
        Debug.Log(string.Format("Output : {0}", _generatedString));

    }

    private string ReplaceRecursive(string prevString, int numIterations)
    {

        if (numIterations == 0) { return prevString; }

        string nextString = "";

        foreach (char c in prevString)
        {
            try
            {
                nextString += _ruleSet._rules[c.ToString()];
            }

            catch //skips the char if not in ruleset
            {
                nextString += c;
            }
        }
        numIterations--;
        return ReplaceRecursive(nextString, numIterations);
    }
}

//rule set class for use in LSystem
public class RuleSet
{
    public string _axiom;
    public Dictionary<string, string> _rules, _terminals;
    public string _alphabet;
    public bool _valid;
    public float _angle;
    public string _category;

    public RuleSet(string category, string axiom, string alphabet, float angle){

        _category = category;
        _axiom = axiom;
        _angle = angle;
        _rules = new Dictionary<string, string>();
        _terminals = new Dictionary<string, string>();
        _alphabet = alphabet;
        _valid = true;
    }

    public void AddRule(string input, string output){

        if(_alphabet.Contains(input)){

            _rules.Add(input, output);

        } else {
            
            Debug.Log("ERROR : " + input + " not in symbol set, rule not added to rule set");
            _valid = false;
        }
    }

    //defines what each symbol maps to in final generation. Every non "F" symbol requires a terminal, otherwise the turtle no comprende
    public void AddTerminal(string input, string output){

        if(_alphabet.Contains(input)){

            _terminals.Add(input, output);

        } else {
            
            Debug.Log("ERROR : " + input + " not in symbol set, rule not added to rule set");
            _valid = false;
        }
    }

    public void Validate(){

        string terminalKeys = _terminals.Keys.ToString();
        string turtleString = "Ff-+|[]!\"";
        char[] turtleChars = turtleString.ToCharArray();

        foreach (char symbol in _alphabet){
            if(!turtleChars.Contains(symbol) && !_terminals.ContainsKey(symbol.ToString())) {
                
                Debug.Log("ERROR : Missing Terminal value for the symbol " + symbol);
                _valid = false;
            }
        }

        foreach (char symbol in _axiom)
        {
            if (!turtleChars.Contains(symbol) && !_alphabet.ToCharArray().Contains(symbol))
            {
                Debug.Log("ERROR : " + _axiom + " not contained in symbol set, please check");
                _valid = false;
            }
        }
    }
}
