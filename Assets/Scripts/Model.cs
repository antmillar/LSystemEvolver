using System.Collections;
using System.Linq;
using UnityEngine;
public class Model
{
    public RuleSet[] _rulesets;
    public Mesh[] meshes;
    public GeneticAlgo gaRules, gaAxioms;
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
        meshes = new Mesh[_childCount];

        //InitialiseDB.Initialise();
        var ldb = new LSystemDB();

        var systemsJSON = ldb.ReadFromFile();

        _sampleRules = new string[_childCount][];
        _sampleAxioms = new string[_childCount][];

        _encode = new Encoder("Ff-+|!£\"GHI^&$");

        //get the L systems from database and assign them to a gameobject which is created here
        for (int i = 0; i < _childCount; i++)
        {
            //get the rule sets for samples from DB
            RuleSet tempRS = new RuleSet(systemsJSON[(i % systemsJSON.Count).ToString()]);
            _rulesets[i] = tempRS;
            meshes[i] = MeshFromRuleset(i);
            EncodeSamples(i);
        }

        gaRules = CreateGA(_sampleRules);
        gaAxioms = CreateGA(_sampleAxioms);
    }

    //encodes the initial sample l systems to genomes
    public void EncodeSamples(int idx)
    {
        //the ordering here matters due to sample rules
        var keyArray = _rulesets[idx]._rules.Keys.ToArray();
        string ruleNames = string.Join("", keyArray);

        _sampleRules[idx] = new string[keyArray.Length];

        for (int i = 0; i < keyArray.Length; i++)
        {
            string rule = _rulesets[idx]._rules[ruleNames[i].ToString()];
            _sampleRules[idx][i] = _encode.Encode(rule);
        }

        //encoding the Axioms

        _sampleAxioms[idx] = new string[1];

        string axiom = _rulesets[idx]._axiom;
        _sampleAxioms[idx][0] = _encode.Encode(axiom);
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

    //converts from genomes to _rulesets
    public void DecodeGenomes()
    {

        for (int i = 0; i < _childCount; i++)
        {
            var temp = gaAxioms.Population._genomes[i]._genome[0];

            string evolvedAxiom = gaAxioms.Encoder.Decode(temp);
            //Debug.Log(evolvedAxiom);

            string ruleNames = "FGHI";
            RuleSet rsTemp = new RuleSet("Fractal", evolvedAxiom, ruleNames, 90f);
            //rsTemp.AddTerminal("G", "F");
            //rsTemp.AddTerminal("H", "F");
            //issue here because the rules aren't attached to their rules the order matters when passing/reversing
            //so all systems currently need to have fgh defined and in order

            for (int j = 0; j < gaRules.Population._genomes[i]._genome.Length; j++)
            {
                var tempGenome = gaRules.Population._genomes[i]._genome[j];
                string specimen = gaRules.Encoder.Decode(tempGenome);
                rsTemp.AddRule(ruleNames[j].ToString(), specimen);
            }

            rsTemp.Validate();
            _rulesets[i] = rsTemp;
        }
    }

    //runs the next generation of the algo and updates the meshes
    public void NextGeneration(string inputSelection)
    {
        //separate GAs for the axiom and rules currently, to maintain separation
        gaRules.NextGeneration(inputSelection);
        gaAxioms.NextGeneration(inputSelection);
        DecodeGenomes();

        for (int i = 0; i < _childCount; i++)
        {
            int j = i;
            meshes[i] = MeshFromRuleset(i);
        }
    }
}
