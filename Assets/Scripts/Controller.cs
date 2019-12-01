using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class Controller : MonoBehaviour {

    Model model;
    View view;

    private void Start () {

        int childCount = 16;
        view = new View(childCount);
        model = new Model(childCount);

        view.MeshesToMeshFilters(model.meshes);
        view.UpdateGuiText(model._rulesets, model.geneticAlgo.Population._generation);
    }

    public void OnClickEvolve()
    {
        NextGeneration();
    }

    //runs the next generation of the algo and updates the meshes
    public void NextGeneration()
    {
        string inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>().text; //should rewrite this in view with parsing

        model.NextGeneration(inputSelection);
        view.MeshesToMeshFilters(model.meshes);
        view.UpdateGuiText(model._rulesets, model.geneticAlgo.Population._generation);
    }

}

//subdivision as rescaling, read catmull clark??
//quad subdivision and apply evo algo
//need to allow rules with input of length 2










