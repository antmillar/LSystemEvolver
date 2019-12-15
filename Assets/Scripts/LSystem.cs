using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LSystem
{
    private string _axiom;
    private string _generatedString;
    private int _numIterations;
    public RuleSet _ruleSet;

    public LSystem(string axiom, int numIterations, RuleSet rs)
    {
        _axiom = axiom;
        _numIterations = numIterations;
        _ruleSet = rs;
    }



    //generates the final string from the ruleset and axiom
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

    //applies rules recursively
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

    //prints the full l system setup to log
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

    //maps the final generation with 'terminals' if needed
    //these are what dummy character map to in the final iteration, e.g G -> F, not used currently
    private void ApplyTerminals()
    {
        foreach (string s in _ruleSet._terminals.Keys)
        {
            _generatedString = _generatedString.Replace(s, _ruleSet._terminals[s]);
        }
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
    public int _ruleCount;

    [JsonConstructor] //identifies to which constructor to use in json deserialisation
    public RuleSet(string category, string axiom, string alphabet, float angle)
    {

        _ruleCount = 0;
        _category = category;
        _axiom = axiom;
        _angle = angle;
        _rules = new Dictionary<string, string>();
        _terminals = new Dictionary<string, string>();
        _alphabet = alphabet;
        _valid = true;
    }

    //copy constructor
    public RuleSet(RuleSet rs)
    {
        _ruleCount = rs._ruleCount;
        _category = rs._category;
        _axiom = rs._axiom;
        _angle = rs._angle;
        _rules = new Dictionary<string, string>(rs._rules);
        _terminals = new Dictionary<string, string>(rs._terminals);
        _alphabet = rs._alphabet;
        _valid = true;
    }

    public void AddRule(string input, string output)
    {

        if (_alphabet.Contains(input))
        {

            _rules.Add(input, output);
            _ruleCount++;
        }
        else
        {

            Debug.Log("ERROR : " + input + " not in symbol set, rule not added to rule set");
            _valid = false;
        }
    }

    //defines what each symbol maps to in final generation, if using dummies
    public void AddTerminal(string input, string output)
    {

        if (_alphabet.Contains(input))
        {

            _terminals.Add(input, output);

        }
        else
        {

            Debug.Log("ERROR : " + input + " not in symbol set, rule not added to rule set");
            _valid = false;
        }
    }

    //confirms whether the ruleset is valid, this still needs work to catch edge cases.
    //should replace debug logs with proper errors catching at some point
    public void Validate()
    {

        string terminalKeys = _terminals.Keys.ToString();
        string turtleString = "Ff-+|!£\"GHI^&$"; //current valid turtle symbols, should really pass this in elsewhere
        char[] turtleChars = turtleString.ToCharArray();

        foreach (char symbol in _alphabet)
        {
            if (!turtleChars.Contains(symbol) && !_terminals.ContainsKey(symbol.ToString()))
            {

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
