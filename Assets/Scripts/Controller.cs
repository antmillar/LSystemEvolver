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
    IEnumerator animationCoroutine;
    bool animationOn;

    private void Awake () {

        Initialise();

        //Unity Recorder requires a software cursor to record it
        var cursorTexture = Resources.Load<Texture2D>("Cursors/arrow-address");
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);

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

        //animates the turtle drawing the mesh
        if(Input.GetKeyDown("r") & view._zoomed == true)
        {
            int objNum = view.GetActiveNumber();

            if (animationOn)
            {
                StopCoroutine(animationCoroutine);
                animationOn = false;
            }
            else
            {
                animationCoroutine = model.AnimateMesh(objNum, view);
                StartCoroutine(animationCoroutine);
                animationOn = true;
            }
        }

        //completes the mesh provided animation not on
        if (Input.GetKeyDown("t") & view._zoomed == true)
        {
            int objNum = view.GetActiveNumber();

            if (animationOn)
            {
                StopCoroutine(animationCoroutine);
                view.MeshRedraw(model.MeshFromRuleset(objNum));
            }

            else
            {
                view.MeshRedraw(model.MeshFromRuleset(objNum));
            }
        }

    }
}











