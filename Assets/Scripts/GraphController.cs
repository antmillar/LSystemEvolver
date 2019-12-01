//using UnityEngine;
//using UnityEngine.UI;
//using System.Linq;
//public class GraphController : MonoBehaviour {

//    RuleSet[] rulesets;
//    InputField inputSelection;
//    Text textGeneration;
//    int imageCount;
//    GeneticAlgo geneticAlgo;
//    Encoder encode;
//    string[] sampleGenomes;

//    private void Start () {

//        imageCount = 16;
//        textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
//        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
//        sampleGenomes = new string[2 * imageCount];
//        encode = new Encoder();
//        rulesets = new RuleSet[imageCount];

//        //InitialiseDB.Initialise();
//        var ldb = new LSystemDB();
//        var systemsJSON = ldb.ReadFromFile();

//        //get the L systems from database and assign them to a gameobject which is created here
//        for (int i = 0; i < imageCount; i++)
//        {
//            AddGuiItem(i);

//            //get the rule sets for s from DB
//            rulesets[i] = systemsJSON[(i % 4).ToString()];
//            MeshFilter mf = GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>();

//            MeshFromRuleset(rulesets[i], mf);
//            EncodeSampleLSystems(i);
//        }

//        CreateGA(sampleGenomes);
//    }

//    public void EncodeSampleLSystems(int idx)
//    {
//        //encodes the chosen lsystem to a genome
//        string ruleF = rulesets[idx]._rules["F"];
//        string ruleG = rulesets[idx]._rules["G"];

//        string genomeF = encode.Encode(ruleF);
//        string genomeG = encode.Encode(ruleG);

//        //need to sort this out
//        sampleGenomes[idx] = genomeF;
//        sampleGenomes[imageCount + idx] = genomeG;

//        Text textCaption = GameObject.Find("TextCaption" + idx.ToString()).GetComponent<Text>();
//        textCaption.text = "F -> " + ruleF + "\n" + "G -> " + ruleG;
//    }

//    //generates a mesh from a l ssz
//    public void MeshFromRuleset(RuleSet ruleSet, MeshFilter mf)
//    {
//        //create L system
//        LSystem ls = new LSystem(ruleSet._axiom, 4, ruleSet);
//        string lSystemOutput = ls.Generate();

//        //use turtle to create mesh of L system
//        Turtle turtle = new Turtle(ruleSet._angle);
//        turtle.Decode(lSystemOutput);
//        turtle.CreateMesh();

//        Mesh mesh = turtle._finalMesh;
//        mf.mesh = mesh;
//        Vector3 bnds = mesh.bounds.size;
//        float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
//        mf.transform.localScale = Vector3.one / (maxBound);
//    }


//    //adds the text captions and buttons to raw images
//    public void AddGuiItem(int idx)
//    {
//        RawImage[] rawImages = GetComponentsInChildren<RawImage>();
//        //Add button to each image
//        rawImages[idx].gameObject.AddComponent<Button>();
//        string imageName = rawImages[idx].gameObject.name;
//        rawImages[idx].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageName));

//        //Add game objects to hold text captions
//        GameObject textObject = new GameObject();
//        textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

//        //Add text to game objects created and format/position
//        Text textCaption = textObject.AddComponent<Text>();
//        textCaption.name = "TextCaption" + idx.ToString();
//        textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
//        textCaption.fontSize = 10;
//        textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
//        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
//        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35f);
//        textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
//        textCaption.rectTransform.localPosition = rawImages[idx].rectTransform.localPosition + new Vector3(0, -90, 0);

//        //add meshfilters/renderers to gameobjects that will hold the meshes
//        MeshFilter mf = GameObject.Find("obj" + idx.ToString()).AddComponent<MeshFilter>();
//        mf.gameObject.AddComponent<MeshRenderer>();
//        mf.gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default")); ;
//    }

//    //sets up the initial GA
//    public void CreateGA(string[] sampleGenomes)
//    {
//        System.Random r = new System.Random(Time.frameCount);

//        string selectType = "god mode";
//        string mutateType = "randomChoice";
//        string crossType = "OnePt";

//        Encoder encoder = new Encoder();
//        Fitness fitness = new Fitness("");
//        Population population = new Population(40, samplePopulation: sampleGenomes, variableGenomeLength: true);
//        Selection selection = new Selection(selectType);
//        CrossOver crossover = new CrossOver(crossType);
//        Mutation mutation = new Mutation(mutateType, 0.1f);

//        geneticAlgo = new GeneticAlgo(encoder, fitness, population, selection, crossover, mutation);
//    }

//    //converts from genomes to rulesets
//    public void UpdateRulesets(int idx)
//    {
//        //convert initial genomes to lsystems
//        string specimenF = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[idx].genome);
//        string specimenG = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[16 + idx].genome);

//        //change the ruleset rules according to evolution
//        var ruleSet = rulesets[idx];
//        ruleSet._rules["F"] = specimenF;
//        ruleSet._rules["G"] = specimenG;

//        GameObject.Find("TextCaption" + idx.ToString()).GetComponent<Text>().text = "F -> " + string.Join("", specimenF) + "\n" + "G -> " + string.Join("", specimenG);
//    }

//    //runs the next generation of the algo and updates the meshes
//    public void NextGeneration()
//    {
//        geneticAlgo.NextGeneration(inputSelection.text);

//        for (int i = 0; i < imageCount; i++)
//        {
//            UpdateRulesets(i);
//            MeshFilter mf = GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>();
//            RuleSet rs = rulesets[i];
//            MeshFromRuleset(rs, mf);
//        }

//        textGeneration.text = "GENERATION : " + geneticAlgo.Population._generation.ToString();
//    }

//    public void OnClickEvolve()
//    {
//        NextGeneration();
//    }

//    public void OnClickImage(string rawSelectionNum)
//    {
//        inputSelection.text = rawSelectionNum.Substring(3).ToString() + " " + inputSelection.text;
//        string[] inputs = inputSelection.text.Split(' ');
//        string[] checkedInputs = inputs.Length > 5 ? inputs.Take(5).ToArray() : inputs;
//        inputSelection.text = string.Join(" ", checkedInputs);
//    }

//    // Update is called once per frame
//    void Update () {
//    }
//}





////subdivision as rescaling, read catmull clark??
////quad subdivision and apply evo algo
////need to allow rules with input of length 2



