using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Controller : MonoBehaviour {

    Model model;
    View view;
    [SerializeField] float _mutationRate;
    [SerializeField] Material _material;

    private void Start () {

        int childCount = 16;
        view = new View(childCount, _material);
        model = new Model(childCount, _mutationRate);
        
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
            view.OnClickRightClick();
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










