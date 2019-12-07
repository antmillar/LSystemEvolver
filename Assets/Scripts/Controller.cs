using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class Controller : MonoBehaviour {

    public Model model;
    View view;
    float _mutationRate = 0.1f;
    int _childCount = 12;
    int _iterationCount = 4;
    [SerializeField] Material _material;
    public static int counter = 0;

    private void Awake () {

        Initialise();
    }

    public void Initialise()
    {
        view = new View(_childCount, _material);
        model = new Model(_childCount, _iterationCount, _mutationRate);

        view.MeshesToMeshFilters(model.meshes);
        view.UpdateGuiText(model._rulesets, model.gaRules.Population._generation);
    }

    public void OnClickEvolve()
    {
        NextGeneration();
    }

    //runs the next generation of the algo and updates the meshes
    public void NextGeneration()
    {
        string inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>().text;

        model.NextGeneration(inputSelection);
        view.MeshesToMeshFilters(model.meshes);
        view.UpdateGuiText(model._rulesets, model.gaRules.Population._generation);
        Debug.Log(model.gaRules.Population._generation);
    }

    public void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            view.OnClickRightClick();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown("space") & view._zoomed == false)
        {

            model = new Model(_childCount, _iterationCount,  _mutationRate);

            view.MeshesToMeshFilters(model.meshes);
            view.UpdateGuiText(model._rulesets, model.gaRules.Population._generation);
        }


        if(Input.GetKeyDown("r") & view._zoomed == true)
        {

            int objNum = view.GetActiveNumber();
            StartCoroutine(model.AnimateMesh(objNum, view));


        }
    }


}

//subdivision as rescaling, read catmull clark??
//quad subdivision and apply evo algo
//need to allow rules with input of length 2

//now it's possible to have two rules but all must have two rules

//get rid of the genes/ char[][]
//random seeds
//can i replace xpt1 and ypt1 with one variable?


//sort out how the genome allowed bases, the rule translation etc interlinked
//add validation for the string of acceptable icons
//remove the terminals and just don't draw with the turtle

//add bracketing to strings
//allow genomes to change number of genes during crossover

//allow the angle to be a rule



//figure out some seed genomes, what are the basic units of a form??










