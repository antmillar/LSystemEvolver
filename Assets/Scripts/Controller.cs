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





    //currently what I am doing?
    //specify 32 genomes (16 * 2)
    //the i create a population of 40
    //this is seeded from the 4 starting l systems using modulo
    //this population has selection applied to it, using the indices of the samples chosen
    //these are then bred/mutated to generate a new population of size 40
    //ONLY the first 32 are displayed....

    //so what do I need to fix?
    //allow different amounts of genomes to be selected (from 1 - 5?)
    //make the population size the same as the genomes needed
    //main issue here is how to  generate the selection pool of correct size from the selected count
    //currently I'm restricted to a divisor, because my population size controls my selectoin pool size (same)

    //populations must always be an even number I should verify that
    //









