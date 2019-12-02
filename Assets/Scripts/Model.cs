using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Model
{
    public RuleSet[] _rulesets;
    public Mesh[] meshes;
    public GeneticAlgo geneticAlgo;
    int _childCount;
    Encoder _encode;
    string[][] _sampleGenomes;

    public Model(int childCount)
    {
        _childCount = childCount;
        _sampleGenomes = new string[_childCount][];
        _encode = new Encoder();
        _rulesets = new RuleSet[_childCount];
        meshes = new Mesh[_childCount];

        InitialiseDB.Initialise();
        var ldb = new LSystemDB();
        
        var systemsJSON = ldb.ReadFromFile();

        //get the L systems from database and assign them to a gameobject which is created here
        for (int i = 0; i < _childCount; i++)
        {
            //get the rule sets for samples from DB
            RuleSet tempRS = new RuleSet(systemsJSON[(i % 5).ToString()]);
            _rulesets[i] = tempRS;
            meshes[i] = MeshFromRuleset(_rulesets[i]);
            EncodeSampleLSystems(i);
        }

        CreateGA(_sampleGenomes);
    }

    //encodes the initial sample l systems to genomes
    public void EncodeSampleLSystems(int idx)
    {
        string ruleNames = "FGH";
        _sampleGenomes[idx] = new string[_rulesets[idx]._ruleCount];

        for (int i = 0; i < _rulesets[idx]._ruleCount; i++)
        {
            string rule = _rulesets[idx]._rules[ruleNames[i].ToString()];
            _sampleGenomes[idx][i] = _encode.Encode(rule);
        }

        Debug.Log(_sampleGenomes[idx].Length);

        //foreach (KeyValuePair<string, string> entry in _rulesets[idx]._rules)
        //{
        //    _tempGenomes.Add(_encode.Encode(entry.Value));

        //    // do something with entry.Value or entry.Key
        //}


        ////encodes the chosen lsystem to a genome
        //string ruleF = _rulesets[idx]._rules["F"];
        //string ruleG = _rulesets[idx]._rules["G"];

        //string genomeF = _encode.Encode(ruleF);
        //string genomeG = _encode.Encode(ruleG);

        ////need to sort this out
        
        //_sampleGenomes[idx][0] = genomeF;
        //_sampleGenomes[idx][1] = genomeG;
    }

    //generates a mesh from a l system ruleset
    public Mesh MeshFromRuleset(RuleSet ruleSet)
    {
        //create L system
        LSystem ls = new LSystem(ruleSet._axiom, 4, ruleSet);
        string lSystemOutput = ls.Generate();

        //use turtle to create mesh of L system
        Turtle turtle = new Turtle(ruleSet._angle);
        turtle.Decode(lSystemOutput);
        turtle.CreateMesh();

        Mesh mesh = turtle._finalMesh;

        return mesh;
    }

    //sets up the initial GA
    public void CreateGA(string[][] _sampleGenomes)
    {
        System.Random r = new System.Random(Time.frameCount);

        string selectType = "god mode";
        string mutateType = "randomChoice";
        string crossType = "OnePt";

        Encoder encoder = new Encoder();
        Fitness fitness = new Fitness("");
        Population population = new Population(16, samplePopulation: _sampleGenomes, variableGenomeLength: true);
        Selection selection = new Selection(selectType);
        CrossOver crossover = new CrossOver(crossType);
        Mutation mutation = new Mutation(mutateType, 0.0f);

        geneticAlgo = new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
    }

    //converts from genomes to _rulesets
    public void DecodeGenomes()
    {
        for (int i = 0; i < _childCount; i++)
        {
            ////convert initial genomes to lsystems
            //var tempGenome0 = geneticAlgo.Population._genomes[i].genome[0];
            //var tempGenome1 = geneticAlgo.Population._genomes[i].genome[1];

            //string specimenF = geneticAlgo.Encoder.Decode(tempGenome0);
            //string specimenG = geneticAlgo.Encoder.Decode(tempGenome1);

            //////change the ruleset rules according to evolution

            //_rulesets[i]._rules["F"] = specimenF;
            //_rulesets[i]._rules["G"] = specimenG;

            //create a new ruleset with all available parameters
            string ruleNames = "FGH";

            //why are the genomes changing in length???
            //need a new function called genomeToRuleset

            RuleSet rsTemp = new RuleSet("Fractal", "F-F-F-F", "FGH", 90f);
            rsTemp.AddTerminal("G", "F");
            rsTemp.AddTerminal("H", "F");

            for (int j = 0; j < geneticAlgo.Population._genomes[i].genome.Length; j++)
            {
                var tempGenome = geneticAlgo.Population._genomes[i].genome[j];
                string specimen = geneticAlgo.Encoder.Decode(tempGenome);
                rsTemp.AddRule(ruleNames[j].ToString(), specimen);
            }

            rsTemp.Validate();
            _rulesets[i] = rsTemp;

        }
    }

    //runs the next generation of the algo and updates the meshes
    public void NextGeneration(string inputSelection)
    {
        geneticAlgo.NextGeneration(inputSelection);
        DecodeGenomes();

        for (int i = 0; i < _childCount; i++)
        {
            int j = i;
            meshes[i] = MeshFromRuleset(_rulesets[j]);
        }

    }






}
