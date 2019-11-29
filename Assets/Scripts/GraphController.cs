using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GraphController : MonoBehaviour {

    Turtle turtle;
    public Material material;
    RuleSet[] rulesets;
    InputField inputSelection;
    RawImage[] rawImages;
    Text textGeneration;

    GeneticAlgo geneticAlgo;

    private void Start () {

        textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
        material = new Material(Shader.Find("Sprites/Default"));
        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
        rawImages = GetComponentsInChildren<RawImage>();

        InitialiseDB.Initialise();

        var test = new LSystemDB();
        var systemsJSON = test.ReadFromFile();

        string[] sampleGenomes = new string[32];
        Encoder encode = new Encoder();
        rulesets = new RuleSet[16];

        //get the L systems from database and assign them to a gameobject which is created here
        for (int i = 0; i < 16; i++)
        {
            ConstructGuiItem(i);

            Text textCaption = GameObject.Find("TextCaption" + i.ToString()).GetComponent<Text>();
            MeshFilter mf = GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>();

            //get the rule sets for sample
            rulesets[i] = systemsJSON[(i % 4).ToString()];

            MeshFromRuleset(rulesets[i], mf);

            //encodes the chosen lsystem to a genome
            string ruleF = rulesets[i]._rules["F"];
            string ruleG = rulesets[i]._rules["G"];

            string genomeF = encode.Encode(ruleF);
            string genomeG = encode.Encode(ruleG);

            sampleGenomes[i] = genomeF;
            sampleGenomes[16 + i] = genomeG;
            
            textCaption.text = "F -> " + ruleF + "\n" + "G -> " + ruleG;
           
        }

        CreateGA(sampleGenomes);
    }

    public void MeshFromRuleset(RuleSet ruleSet, MeshFilter mf)
    {
        //create L system
        LSystem ls = new LSystem(ruleSet._axiom, 4, ruleSet);
        string lSystemOutput = ls.Generate();

        //use turtle to create mesh of L system
        turtle = new Turtle(ruleSet._angle);
        turtle.Decode(lSystemOutput);
        turtle.CreateMesh();

        Mesh mesh = turtle._finalMesh;
        mf.mesh = mesh;
        Vector3 bnds = mesh.bounds.size;
        float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
        mf.transform.localScale = Vector3.one / (maxBound);
    }

    public void ConstructGuiItem(int idx)
    {
        //Add button to each image
        rawImages[idx].gameObject.AddComponent<Button>();
        string imageName = rawImages[idx].gameObject.name;
        rawImages[idx].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageName));

        //Add game objects to hold text captions
        GameObject textObject = new GameObject();
        textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

        //Add text to game objects created and format/position
        Text textCaption = textObject.AddComponent<Text>();
        textCaption.name = "TextCaption" + idx.ToString();
        textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
        textCaption.fontSize = 10;
        textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35f);
        textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
        textCaption.rectTransform.localPosition = rawImages[idx].rectTransform.localPosition + new Vector3(0, -90, 0);

        //add meshfilters/renderers to gameobjects that will hold the meshes
        MeshFilter mf = GameObject.Find("obj" + idx.ToString()).AddComponent<MeshFilter>();
        mf.gameObject.AddComponent<MeshRenderer>();
        mf.gameObject.GetComponent<Renderer>().material = material;
    }

    public void CreateGA(string[] sampleGenomes)
    {
        System.Random r = new System.Random(Time.frameCount);

        string selectType = "god mode";
        string mutateType = "randomChoice";
        string crossType = "OnePt";

        Encoder encoder = new Encoder();
        Fitness fitness = new Fitness("");
        Population population = new Population(40, samplePopulation: sampleGenomes, variableGenomeLength: true);
        Selection selection = new Selection(selectType);
        CrossOver crossover = new CrossOver(crossType);
        Mutation mutation = new Mutation(mutateType, 0.1f);

        geneticAlgo = new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
    }

    public void UpdateRulesets(int idx)
    {
        //convert initial genomes to lsystems
        string specimenF = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[idx].genome);
        string specimenG = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[16 + idx].genome);

        //change the ruleset rules according to evolution
        var ruleSet = rulesets[idx];
        ruleSet._rules["F"] = specimenF;
        ruleSet._rules["G"] = specimenG;

        GameObject.Find("TextCaption" + idx.ToString()).GetComponent<Text>().text = "F -> " + string.Join("", specimenF) + "\n" + "G -> " + string.Join("", specimenG);
    }

    public void DisplayPhenotypes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UpdateRulesets(i);
            MeshFilter mf = GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>();
            RuleSet rs = rulesets[i];
            MeshFromRuleset(rs, mf);
        }
     }

    public void OnButtonClick()
    {
        geneticAlgo.NextGeneration(inputSelection.text);
        DisplayPhenotypes(16);



        textGeneration.text = "GENERATION : " + geneticAlgo.Population._generation.ToString();
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
//quad subdivision and apply evo algo
//need to allow rules with input of length 2



