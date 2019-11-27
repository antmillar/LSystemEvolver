using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GraphController : MonoBehaviour {

    // Use this for initialization
    Turtle turtle;
    public Material material;
    MeshFilter mf;
    MeshRenderer mr;
    RuleSet[] rulesets;
    MeshFilter[] meshFilters;
    InputField inputSelection;
    RawImage[] rawImages;
    Text textGeneration;

    GeneticAlgo geneticAlgo;

    private void Start () {

        textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
        material = new Material(Shader.Find("Sprites/Default"));
        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
        string systemchoice = gameObject.GetComponent<Text>().text;
        rawImages = GetComponentsInChildren<RawImage>();

        InitialiseDB.Initialise();

        var test = new LSystemDB();
        var systemsJSON = test.ReadFromFile();

        meshFilters = new MeshFilter[5];
        string[] sampleGenomes = new string[5];
        Encoder encode = new Encoder();
        rulesets = new RuleSet[5];

        //get the L systems from database and assign them to a gameobject which is created here
        for (int i = 0; i < 5; i++)
        {
            rawImages[i].gameObject.AddComponent<Button>();
            Button btn = rawImages[i].gameObject.GetComponent<Button>();
            string txt = rawImages[i].gameObject.name;
            btn.onClick.AddListener(() => OnClickImage(txt));

            Debug.Log("System " + i.ToString());
            var rsTest = systemsJSON[(i % 3).ToString()];
            rulesets[i] = rsTest;
            //create L system
            LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
		    string lSystemOutput = ls.Generate();
		    ls.Information();
		
		    //use turtle to create mesh of L system
		    turtle = new Turtle(rsTest._angle);
		    turtle.Decode(lSystemOutput);
            turtle.DrawMesh();

            meshFilters[i] = GameObject.Find("obj" + i.ToString()).AddComponent<MeshFilter>();
            meshFilters[i].gameObject.AddComponent<MeshRenderer>();

            meshFilters[i].gameObject.GetComponent<Renderer>().material = material;
            Mesh mesh = turtle._finalMesh;
            meshFilters[i].mesh = mesh;
            Vector3 bnds = mesh.bounds.size;
            float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
            meshFilters[i].transform.localScale = Vector3.one / maxBound;
  
            //encodes the chosen lsystem to a genome
 
            string rule = rsTest._rules["F"];

            char[] chars = rule.ToCharArray();
            string[] ruleStrings = new string[chars.Length];
            for(int j = 0; j < chars.Length; j++)
            {
                ruleStrings[j] = chars[j].ToString();
            }
            string genome = encode.Encode(ruleStrings);
            sampleGenomes[i] = genome;
            Debug.Log(sampleGenomes[i]);
        }
        //LIMIT FRACTAL LENGTH!!!!!
        CreateGA(sampleGenomes);
    }


    public void CreateGA(string[] sampleGenomes)
    {
        System.Random r = new System.Random(Time.frameCount);

        string selectType = "god mode";
        string mutateType = "randomChoice";
        string crossType = "OnePt";

        Encoder encoder = new Encoder();
        Fitness fitness = new Fitness("");
        Population population = new Population(10, samplePopulation: sampleGenomes);
        Selection selection = new Selection(selectType);
        CrossOver crossover = new CrossOver(crossType);
        Mutation mutation = new Mutation(mutateType);

        geneticAlgo = new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
    }

    public void DisplayPhenotypes(int count)
    {

        for (int i = 0; i < count; i++)
        {
            //convert initial genomes to lsystems
            string[] specimen = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[i].genome);
            Debug.Log(string.Join("", specimen));

            var rsTest = rulesets[i];
            rsTest._rules["F"] = string.Join("", specimen);

            LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
            string lSystemOutput = ls.Generate();

            //use turtle to create mesh of L system
            turtle = new Turtle(rsTest._angle);
            turtle.Decode(lSystemOutput);
            turtle.DrawMesh();

            Mesh mesh = turtle._finalMesh;
            GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>().mesh = mesh;
            Vector3 bnds = mesh.bounds.size;
            float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
            GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>().transform.localScale = Vector3.one / maxBound;
        }
     }

    public void OnButtonClick()
    {
        geneticAlgo.NextGeneration(inputSelection.text);
        Debug.Log("GENERATION : " + geneticAlgo.Population._generation.ToString());
        DisplayPhenotypes(5);
        textGeneration.text = "Generation #" + geneticAlgo.Population._generation.ToString();
    }

    public void OnClickImage(string rawSelectionNum)
    {
        inputSelection.text = rawSelectionNum.Substring(3).ToString() + " " + inputSelection.text;
        CheckInputCount();
    }

    public void CheckInputCount()
    {
        string[] inputs = inputSelection.text.Split(' ');
        string[] checkedInputs = inputs.Length > 5 ? inputs.Take(5).ToArray() : inputs;
        inputSelection.text = string.Join(" ", checkedInputs);
    }

    // Update is called once per frame
    void Update () {
    }
}

//subdivision as rescaling, read catmull clark??
//evolve an L system that approximates a line?
//quad subdivision and apply evo algo
//need to allow rules with input of length 2



