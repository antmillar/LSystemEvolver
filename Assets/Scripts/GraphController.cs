using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour {

    // Use this for initialization
    Turtle turtle;
    public Material material;
    MeshFilter mf;
    MeshRenderer mr;

    private void Start () {

        string systemchoice = gameObject.GetComponent<Text>().text;

        InitialiseDB.Initialise();

        var test = new LSystemDB();
        var systemsJSON = test.ReadFromFile();

        MeshFilter[] mfs = new MeshFilter[3];

        for (int i = 1; i < 4; i++)
        {
        Debug.Log("System " + i.ToString());
        var rsTest = systemsJSON["0"];
		
		//create L system
		LSystem ls = new LSystem(rsTest._axiom, 4, rsTest);
		string lSystemOutput = ls.Generate();
		ls.Information();
		
		//use turtle to create mesh of L system
		turtle = new Turtle(rsTest._angle);
		turtle.Decode(lSystemOutput);
        turtle.DrawMesh();

        mfs[i - 1] = GameObject.Find("obj" + i.ToString()).AddComponent<MeshFilter>();
        mfs[i - 1].gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = turtle._finalMesh;
        mfs[i - 1].mesh = mesh;
        Vector3 bnds = mesh.bounds.size;
        float maxBound = Mathf.Max(bnds[0], bnds[1], bnds[2]);
        mfs[i - 1].transform.localScale = Vector3.one / maxBound;
            //need to recenter as well probably....
            //need to stop fractals going on too long too

            LEncoder lencode = new LEncoder();
            string temp = rsTest._rules["F"];

            char[] chars = temp.ToCharArray();
            string[] temps = new string[chars.Length];
            for(int j = 0; j < chars.Length; j++)
            {
                temps[j] = chars[j].ToString();
            }
            lencode.Encode(temps);
        }

    }

	// Update is called once per frame
	void Update () {
        //Graphics.DrawMesh(turtle._finalMesh, Matrix4x4.identity, material, 0);
    }
}


//how do i scale the image output
//how do i draw it the mesh on an image?



//subdivision as rescaling, read catmull clark??
//evolve an L system that approximates a line?
//database of systems, with classification??
//quad subdivision and apply evo algo
//need to allow rules with input of length 2


//next steps, evolve a rule
//allow user choice in evo algo
//tidy up evo algo
//populate more L systems from the book, 20??



