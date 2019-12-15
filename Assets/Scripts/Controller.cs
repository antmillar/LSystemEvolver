using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{

    Model _model;
    View _view;
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
        //var cursorTexture = Resources.Load<Texture2D>("Cursors/arrow-address");
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void Initialise()
    {
        _view = new View(_childCount, _material);
        _model = new Model(_childCount, _iterationCount, _mutationRate);

        _view.MeshesToMeshFilters(_model._meshes);
        _view.UpdateGuiText(_model._rulesets, _model._gaRules.Population._generation);
    }

    public void OnClickEvolve()
    {
        NextGeneration();
    }

    //runs the next generation of the algo and updates the _meshes
    public void NextGeneration()
    {
        string inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>().text;

        _model.NextGeneration(inputSelection);
        _view.MeshesToMeshFilters(_model._meshes);
        _view.UpdateGuiText(_model._rulesets, _model._gaRules.Population._generation);
    }

    public void Update()
    {
        //right click set up
        if (Input.GetMouseButtonDown(1))
            _view.OnClickRightClick();

        //escape quit app
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //reset GA on space
        if (Input.GetKeyDown("space") & _view._zoomed == false)
        {
            _model = new Model(_childCount, _iterationCount, _mutationRate);
            _view.MeshesToMeshFilters(_model._meshes);
            _view.UpdateGuiText(_model._rulesets, _model._gaRules.Population._generation);
        }

        //turtle animation toggle
        if (Input.GetKeyDown("r") & _view._zoomed == true)
        {
            int objNum = _view.GetActiveNumber(); //get active _view number

            if (_animationOn)
            {
                StopCoroutine(_animationCoroutine);
                _animationOn = false;
            }
            else
            {
                _animationCoroutine = _model.AnimateMesh(objNum, _view);
                StartCoroutine(_animationCoroutine);
                _animationOn = true;
            }
        }

        //completes the mesh provided animation not on
        if (Input.GetKeyDown("t") & _view._zoomed == true)
        {
            int objNum = _view.GetActiveNumber();

            if (_animationOn)
            {
                StopCoroutine(_animationCoroutine);
                _view.MeshRedraw(_model.MeshFromRuleset(objNum));
            }

            else
            {
                _view.MeshRedraw(_model.MeshFromRuleset(objNum));
            }
        }

    }
}











