//using UnityEngine;
//using UnityEngine.UI;
//using System.Linq;
//public class UserInterface
//{

//    Turtle turtle;
//    public Material material;
//    RuleSet[] rulesets;
//    InputField inputSelection;
//    RawImage[] rawImages;
//    Text textGeneration;

//    GeneticAlgo geneticAlgo;

//    private void Awake()
//    {

//        textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
//        material = new Material(Shader.Find("Sprites/Default"));
//        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
//        string systemchoice = gameObject.GetComponent<Text>().text;
//        rawImages = GameObject.Find("Canvas").GetComponentsInChildren<RawImage>();

//        InitialiseDB.Initialise();

//        var test = new LSystemDB();
//        var systemsJSON = test.ReadFromFile();

//        string[] sampleGenomes = new string[32];
//        Encoder encode = new Encoder();
//        rulesets = new RuleSet[16];

//        //get the L systems from database and assign them to a gameobject which is created here
//        for (int i = 0; i < 16; i++)
//        {
//            //setting up the GUI
//            rawImages[i].gameObject.AddComponent<Button>();
//            GameObject textObject = new GameObject();
//            Text textCaption = textObject.AddComponent<Text>();
//            textCaption.name = "TextCaption" + i.ToString();
//            textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

//            textCaption.color = Color.white;
//            textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
//            textCaption.fontSize = 10;
//            textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
//            textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
//            textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35f);
//            textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
//            textCaption.rectTransform.localPosition = rawImages[i].rectTransform.localPosition + new Vector3(0, -90, 0);

//            //add listener to the buttons
//            Button btn = rawImages[i].gameObject.GetComponent<Button>();
//            string txt = rawImages[i].gameObject.name;
//            btn.onClick.AddListener(() => OnClickImage(txt));

//            //get the rule sets for sample
//            var rsTest = systemsJSON[(i % 4).ToString()];
//            rulesets[i] = rsTest;

//            //create L system
//            LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
//            string lSystemOutput = ls.Generate();
//            ls.Information();

//            //use turtle to create mesh of L system
//            turtle = new Turtle(rsTest._angle);
//            turtle.Decode(lSystemOutput);
//            turtle.CreateMesh();

//            MeshFilter mf = GameObject.Find("obj" + i.ToString()).AddComponent<MeshFilter>();
//            mf.gameObject.AddComponent<MeshRenderer>();

//            mf.gameObject.GetComponent<Renderer>().material = material;
//            Mesh mesh = turtle._finalMesh;
//            mf.mesh = mesh;
//            Vector3 bnds = mesh.bounds.size;
//            float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
//            mf.transform.localScale = Vector3.one / (maxBound);

//            //encodes the chosen lsystem to a genome
//            string ruleF = rsTest._rules["F"];
//            string ruleG = rsTest._rules["G"];

//            char[] charsF = ruleF.ToCharArray();
//            char[] charsG = ruleG.ToCharArray();

//            string[] ruleStringsF = new string[charsF.Length];
//            for (int j = 0; j < charsF.Length; j++)
//            {
//                ruleStringsF[j] = charsF[j].ToString();
//            }

//            string[] ruleStringsG = new string[charsG.Length];
//            for (int j = 0; j < charsG.Length; j++)
//            {
//                ruleStringsG[j] = charsG[j].ToString();
//            }
//            string genomeF = encode.Encode(ruleStringsF);
//            string genomeG = encode.Encode(ruleStringsG);
//            sampleGenomes[i] = genomeF;
//            sampleGenomes[16 + i] = genomeG;
//            textCaption.text = "F -> " + string.Join("", ruleStringsF) + "\n" + "G -> " + string.Join("", ruleStringsG);

//        }
//        //LIMIT FRACTAL LENGTH!!!!!
//        CreateGA(sampleGenomes);
//    }

//    public void PopulateGUI(int popCount)
//    {
//        for (int i = 0; i < popCount; i++)
//        {
//            //Add button to each image
//            rawImages[i].gameObject.AddComponent<Button>();
//            string imageName = rawImages[i].gameObject.name;
//            rawImages[i].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageName));

//            //Add game objects to hold text captions
//            GameObject textObject = new GameObject();
//            textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

//            //Add text to game objects created and format/position
//            Text textCaption = textObject.AddComponent<Text>();
//            textCaption.name = "TextCaption" + i.ToString();
//            textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
//            textCaption.fontSize = 10;
//            textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
//            textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
//            textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35f);
//            textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
//            textCaption.rectTransform.localPosition = rawImages[i].rectTransform.localPosition + new Vector3(0, -90, 0);

//            //add meshfilters/renderers to gameobjects that will hold the meshes
//            MeshFilter mf = GameObject.Find("obj" + i.ToString()).AddComponent<MeshFilter>();
//            mf.gameObject.AddComponent<MeshRenderer>();
//            mf.gameObject.GetComponent<Renderer>().material = material;

//        }
//    }
//            //get the rule sets for sample
//            var rsTest = systemsJSON[(i % 4).ToString()];
//            rulesets[i] = rsTest;

//            //create L system
//            LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
//            string lSystemOutput = ls.Generate();
//            ls.Information();

//            //use turtle to create mesh of L system
//            turtle = new Turtle(rsTest._angle);
//            turtle.Decode(lSystemOutput);
//            turtle.CreateMesh();


//            Mesh mesh = turtle._finalMesh;
//            mf.mesh = mesh;
//            Vector3 bnds = mesh.bounds.size;
//            float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
//            mf.transform.localScale = Vector3.one / (maxBound);

//            //encodes the chosen lsystem to a genome
//            string ruleF = rsTest._rules["F"];
//            string ruleG = rsTest._rules["G"];

//            char[] charsF = ruleF.ToCharArray();
//            char[] charsG = ruleG.ToCharArray();

//            string[] ruleStringsF = new string[charsF.Length];
//            for (int j = 0; j < charsF.Length; j++)
//            {
//                ruleStringsF[j] = charsF[j].ToString();
//            }

//            string[] ruleStringsG = new string[charsG.Length];
//            for (int j = 0; j < charsG.Length; j++)
//            {
//                ruleStringsG[j] = charsG[j].ToString();
//            }
//            string genomeF = encode.Encode(ruleStringsF);
//            string genomeG = encode.Encode(ruleStringsG);
//            sampleGenomes[i] = genomeF;
//            sampleGenomes[16 + i] = genomeG;
//            textCaption.text = "F -> " + string.Join("", ruleStringsF) + "\n" + "G -> " + string.Join("", ruleStringsG);

//        }


//    }
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

//    public void DisplayPhenotypes(int count)
//    {

//        for (int i = 0; i < count; i++)
//        {
//            //convert initial genomes to lsystems
//            string[] specimenF = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[i].genome);
//            string[] specimenG = geneticAlgo.Encoder.Decode(geneticAlgo.Population._genomes[16 + i].genome);
//            var rsTest = rulesets[i];

//            rsTest._rules["F"] = string.Join("", specimenF);
//            rsTest._rules["G"] = string.Join("", specimenG);

//            LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
//            string lSystemOutput = ls.Generate();

//            //use turtle to create mesh of L system
//            turtle = new Turtle(rsTest._angle);
//            turtle.Decode(lSystemOutput);
//            turtle.CreateMesh();

//            Mesh mesh = turtle._finalMesh;
//            GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>().mesh = mesh;
//            Vector3 bnds = mesh.bounds.size;
//            Vector3 firstVertex = mesh.vertices[0];
//            float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
//            //GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>().transform.Translate(mesh.bounds.center);
//            //GameObject.Find("cam" + i.ToString()).GetComponent<Transform>().transform.position = GameObject.Find("obj" + i.ToString()).GetComponent<Transform>().transform.position - mesh.bounds.center + new Vector3(0,0,-1);
//            GameObject.Find("obj" + i.ToString()).GetComponent<MeshFilter>().transform.localScale = Vector3.one / (maxBound);
//            GameObject.Find("TextCaption" + i.ToString()).GetComponent<Text>().text = "F -> " + string.Join("", specimenF) + "\n" + "G -> " + string.Join("", specimenG);
//        }
//    }

//    public void OnButtonClick()
//    {
//        geneticAlgo.NextGeneration(inputSelection.text);
//        Debug.Log("GENERATION : " + geneticAlgo.Population._generation.ToString());
//        DisplayPhenotypes(16);
//        textGeneration.text = "GENERATION : " + geneticAlgo.Population._generation.ToString();
//    }

//    public void OnClickImage(string rawSelectionNum)
//    {
//        inputSelection.text = rawSelectionNum.Substring(3).ToString() + " " + inputSelection.text;
//        CheckInputCount();
//    }

//    public void CheckInputCount()
//    {
//        string[] inputs = inputSelection.text.Split(' ');
//        string[] checkedInputs = inputs.Length > 5 ? inputs.Take(5).ToArray() : inputs;
//        inputSelection.text = string.Join(" ", checkedInputs);
//    }


//}

////subdivision as rescaling, read catmull clark??
////evolve an L system that approximates a line?
////quad subdivision and apply evo algo
////need to allow rules with input of length 2





