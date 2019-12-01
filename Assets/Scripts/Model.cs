using UnityEngine;
using UnityEngine.UI;

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
            _rulesets[i] = systemsJSON[(i % 4).ToString()];
            meshes[i] = MeshFromRuleset(_rulesets[i]);
            EncodeSampleLSystems(i);
        }

        CreateGA(_sampleGenomes);
    }

    //encodes the initial sample l systems to genomes
    public void EncodeSampleLSystems(int idx)
    {
        //encodes the chosen lsystem to a genome
        string ruleF = _rulesets[idx]._rules["F"];
        string ruleG = _rulesets[idx]._rules["G"];

        string genomeF = _encode.Encode(ruleF);
        string genomeG = _encode.Encode(ruleG);

        //need to sort this out
        _sampleGenomes[idx] = new string[2];
        _sampleGenomes[idx][0] = genomeF;
        _sampleGenomes[idx][1] = genomeG;
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
        Mutation mutation = new Mutation(mutateType, 0.5f);

        geneticAlgo = new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
    }

    //converts from genomes to _rulesets
    public void Update_rulesets(int idx)
    {
        //convert initial genomes to lsystems
        string specimenF = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[idx].genome[0]);
        string specimenG = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[idx].genome[1]);

        //change the ruleset rules according to evolution
        var ruleSet = _rulesets[idx];
        ruleSet._rules["F"] = specimenF;
        ruleSet._rules["G"] = specimenG;

        GameObject.Find("TextCaption" + idx.ToString()).GetComponent<Text>().text = "F -> " + string.Join("", specimenF) + "\n" + "G -> " + string.Join("", specimenG);
    }

    //runs the next generation of the algo and updates the meshes
    public void NextGeneration(string inputSelection)
    {
        geneticAlgo.NextGeneration(inputSelection);

        for (int i = 0; i < _childCount; i++)
        {
            Update_rulesets(i);
            meshes[i] = MeshFromRuleset(_rulesets[i]);
        }

    }






}
