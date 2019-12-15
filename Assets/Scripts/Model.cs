using System.Collections;
using System.Linq;
using UnityEngine;

public class Model
{
    public RuleSet[] _rulesets;
    public Mesh[] _meshes;
    public GeneticAlgo _gaRules, _gaAxioms;
    int _childCount, _iterationCount;
    Encoder _encode;
    string[][] _sampleRules, _sampleAxioms;
    float _mutationRate;

    public Model(int childCount, int iterationCount, float mutationRate)
    {
        _mutationRate = mutationRate;
        _childCount = childCount;
        _iterationCount = iterationCount;

        _rulesets = new RuleSet[_childCount];
        _meshes = new Mesh[_childCount];

        //InitialiseDB.Initialise(); //can be used to populate a new set of initial L systems in the DB (specified in LSystemDB.cs)
        var ldb = new LSystemDB();
        var systemsJSON = ldb.ReadFromFile();

        _sampleRules = new string[_childCount][];
        _sampleAxioms = new string[_childCount][];

        _encode = new Encoder("Ff-+|!£\"GHI^&$");

        //get the L systems from database and assign them to as meshes to GOs
        for (int i = 0; i < _childCount; i++)
        {
            RuleSet tempRS = new RuleSet(systemsJSON[(i % systemsJSON.Count).ToString()]);
            _rulesets[i] = tempRS;
            _meshes[i] = MeshFromRuleset(i);
            EncodeSamples(i);
        }

        //one GA for the rules and axioms so they don't get jumbled up
        _gaRules = CreateGA(_sampleRules);
        _gaAxioms = CreateGA(_sampleAxioms);
    }

    //encodes the initial sample L systems to genomes
    public void EncodeSamples(int idx)
    {
        //the ordering here matters currently due to sample rules
        var keyArray = _rulesets[idx]._rules.Keys.ToArray();
        _sampleRules[idx] = new string[keyArray.Length];

        for (int i = 0; i < keyArray.Length; i++)
        {
            string rule = _rulesets[idx]._rules[keyArray[i].ToString()];
            _sampleRules[idx][i] = _encode.Encode(rule);
        }

        //encoding the Axioms
        string axiom = _rulesets[idx]._axiom;
        _sampleAxioms[idx] = new string[1];
        _sampleAxioms[idx][0] = _encode.Encode(axiom);
    }

    //converts from genomes to _rulesets
    public void DecodeGenomes()
    {
        for (int i = 0; i < _childCount; i++)
        {
            var temp = _gaAxioms.Population._genomes[i]._genome[0];

            string evolvedAxiom = _gaAxioms.Encoder.Decode(temp);

            //issue here because the rule keys aren't attached to their rules so the order matters when passing/reversing
            //so all systems currently need to have fghi defined in that order
            string ruleNames = "FGHI";
            RuleSet rsTemp = new RuleSet("Fractal", evolvedAxiom, ruleNames, 90f);

            for (int j = 0; j < _gaRules.Population._genomes[i]._genome.Length; j++)
            {
                var tempGenome = _gaRules.Population._genomes[i]._genome[j];
                string specimen = _gaRules.Encoder.Decode(tempGenome);
                rsTemp.AddRule(ruleNames[j].ToString(), specimen);
            }

            rsTemp.Validate();
            _rulesets[i] = rsTemp;
        }
    }

    //generates a mesh from a l system ruleset
    public Mesh MeshFromRuleset(int idx)
    {
        //create L system
        LSystem ls = new LSystem(_rulesets[idx]._axiom, _iterationCount, _rulesets[idx]);
        string lSystemOutput = ls.Generate();

        //use turtle to create mesh of L system
        Turtle turtle = new Turtle(_rulesets[idx]._angle);
        turtle.Decode(lSystemOutput);
        turtle.CreateMesh();

        Mesh mesh = turtle._finalMesh;

        return mesh;
    }

    //animates the step by step drawing of the mesh
    public IEnumerator AnimateMesh(int objNum, View view)
    {
        var ruleSet = _rulesets[objNum];
        LSystem ls = new LSystem(ruleSet._axiom, _iterationCount, ruleSet);
        string lSystemOutput = ls.Generate();

        //use turtle to create mesh of L system
        Turtle turtle = new Turtle(ruleSet._angle);
        turtle.Decode(lSystemOutput);

        for (int i = 0; i < turtle._meshes.Count; i++)
        {
            turtle.PartialMesh(i);

            //view.InstructionsRewrite(turtle._partialInstructions); //writes the turtle instructions to screen (used in video)
            view.MeshRedraw(turtle._partialMesh); //redraws the partial mesh
            yield return new WaitForSeconds(0.01f);
        }
    }

    //sets up the initial GA
    public GeneticAlgo CreateGA(string[][] _sampleGenomes)
    {
        System.Random r = new System.Random(Time.frameCount);
        //specify the numbers of genes to the population.

        string selectType = "god mode";
        string mutateType = "randomChoice";
        string crossType = "OnePt";

        Encoder encoder = _encode;
        Fitness fitness = new Fitness("");
        Population population = new Population(_childCount, encoder._symbols, samplePopulation: _sampleGenomes, variableGenomeLength: true);
        Selection selection = new Selection(selectType);
        CrossOver crossover = new CrossOver(crossType);
        Mutation mutation = new Mutation(mutateType, _mutationRate);

        return new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
    }

    //runs the next generation of the GA and updates the meshes
    public void NextGeneration(string inputSelection)
    {
        //separate GAs for the axiom and rules currently, to maintain separation
        _gaRules.NextGeneration(inputSelection);
        _gaAxioms.NextGeneration(inputSelection);
        DecodeGenomes();

        for (int i = 0; i < _childCount; i++)
        {
            _meshes[i] = MeshFromRuleset(i);
        }
    }
}


//Notes for improvements
//Encode the L Systems in a different way to keep axiom and rules tied together, and prevent the need to adhere to rules ordering
