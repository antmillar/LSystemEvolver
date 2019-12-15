using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{

    Model model;
    View view;
    float _mutationRate = 0.1f;
    int _childCount = 12;
    int _iterationCount = 4;
    [SerializeField] Material _material;
    IEnumerator _animationCoroutine;
    bool _animationOn;

    private void Awake()
    {

        Initialise();

        //Unity Recorder requires a software cursor to record it
        var cursorTexture = Resources.Load<Texture2D>("Cursors/arrow-address");
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void Initialise()
    {
        view = new View(_childCount, _material);
        model = new Model(_childCount, _iterationCount, _mutationRate);

        view.MeshesToMeshFilters(model._meshes);
        view.UpdateGuiText(model._rulesets, model._gaRules.Population._generation);
    }

    public void OnClickEvolve()
    {
        NextGeneration();
    }

    //runs the next generation of the algo and updates the _meshes
    public void NextGeneration()
    {
        string inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>().text;

        model.NextGeneration(inputSelection);
        view.MeshesToMeshFilters(model._meshes);
        view.UpdateGuiText(model._rulesets, model._gaRules.Population._generation);
    }

    public void Update()
    {
        //right click set up
        if (Input.GetMouseButtonDown(1))
            view.OnClickRightClick();

        //escape quit app
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //reset GA on space
        if (Input.GetKeyDown("space") & view._zoomed == false)
        {
            model = new Model(_childCount, _iterationCount, _mutationRate);
            view.MeshesToMeshFilters(model._meshes);
            view.UpdateGuiText(model._rulesets, model._gaRules.Population._generation);
        }

        //turtle animation toggle
        if (Input.GetKeyDown("r") & view._zoomed == true)
        {
            int objNum = view.GetActiveNumber(); //get active view number

            if (_animationOn)
            {
                StopCoroutine(_animationCoroutine);
                _animationOn = false;
            }
            else
            {
                _animationCoroutine = model.AnimateMesh(objNum, view);
                StartCoroutine(_animationCoroutine);
                _animationOn = true;
            }
        }

        //completes the mesh provided animation not on
        if (Input.GetKeyDown("t") & view._zoomed == true)
        {
            int objNum = view.GetActiveNumber();

            if (_animationOn)
            {
                StopCoroutine(_animationCoroutine);
                view.MeshRedraw(model.MeshFromRuleset(objNum));
            }

            else
            {
                view.MeshRedraw(model.MeshFromRuleset(objNum));
            }
        }

    }
}











