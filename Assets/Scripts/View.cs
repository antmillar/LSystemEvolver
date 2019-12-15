using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class View
{
    MeshFilter[] _meshFilters;
    RawImage[] _rawImages;
    GameObject[] _lights, _objs, _rObjs;
    Camera[] _cams;
    Text[] _captions;
    Material _material;
    RectTransform _canvasInfo;
    InputField _inputSelection;
    Camera _mainCam, _activeCam;
    RenderTexture _activeRenderTexture;
    GameObject _activeObj;

    int _childCount;
    public bool _zoomed;

    public View(int childCount, Material material)
    {
        //settings
        _childCount = childCount;
        _zoomed = false;
        _material = material;

        //retrieving objects from scene
        _inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
        _mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        _rawImages = GameObject.Find("Canvas").GetComponentsInChildren<RawImage>();
        _canvasInfo = GameObject.Find("CanvasInfo").GetComponent<RectTransform>();

        //arrays to hold assets
        _meshFilters = new MeshFilter[_childCount];
        _lights = new GameObject[_childCount];
        _cams = new Camera[_childCount];
        _objs = new GameObject[_childCount];
        _rObjs = new GameObject[_childCount];
        _captions = new Text[_childCount];

        PopulateGui();
    }

    #region GUI initialisation scripts
    //adds all the items to the gui
    public void PopulateGui()
    {
        for (int i = 0; i < _childCount; i++)
        {
            AddObject(i);
            AddCamera(i);
            AddImageButton(i);
            AddCaption(i);
            AddMeshToObj(i);
            AddLight(i);
            AddRotationScript(i);
        }
    }

    //add object and object to rotate around, to assign a mesh to later
    public void AddObject(int idx)
    {
        GameObject rotateObj = new GameObject("rObj" + idx);
        GameObject newObj = new GameObject("obj" + idx);

        rotateObj.transform.SetParent(GameObject.Find("Individuals").gameObject.GetComponent<Transform>(), true);
        rotateObj.transform.localPosition = new Vector3(250 * (idx + 1), 0, 2); //z offset 2 from camera

        newObj.transform.parent = rotateObj.transform;
        newObj.transform.localPosition = Vector3.zero;
        newObj.transform.localEulerAngles = new Vector3(15, 0, 0);

        _rObjs[idx] = rotateObj;
        _objs[idx] = newObj;
    }

    //add camera for each object and assign render texture to it to output to the GUI 
    public void AddCamera(int idx)
    {
        Camera newCam = new GameObject("cam" + idx).AddComponent<Camera>();
        newCam.transform.position = new Vector3(250 * (idx + 1), 0, 0);
        newCam.targetTexture = Resources.Load<RenderTexture>("Materials/rendText" + idx.ToString());
        newCam.transform.SetParent(GameObject.Find("Individuals").gameObject.GetComponent<Transform>(), true);

        _cams[idx] = newCam;
    }

    //adds l system information caption under raw images
    public void AddCaption(int idx)
    {
        //Add game objects to hold text captions
        GameObject textObject = new GameObject();
        RectTransform parentImage = _rawImages[idx].rectTransform;
        textObject.transform.SetParent(parentImage);

        //Add text to game objects created and format/position
        Text textCaption = textObject.AddComponent<Text>();
        textCaption.name = "TextCaption" + idx.ToString();
        textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
        textCaption.fontSize = 6;
        textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentImage.rect.width);
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentImage.rect.height * 0.25f);
        textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
        textCaption.rectTransform.localPosition = new Vector3(0, -0.5f * parentImage.rect.height, 0); //offset of the caption from image

        textCaption.gameObject.AddComponent<Button>();
        textCaption.gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickFocus(idx));
        _captions[idx] = textCaption;
    }

    //adds the button to raw image for interaction
    public void AddImageButton(int idx)
    {
        Button btnImage = _rawImages[idx].gameObject.AddComponent<Button>();

        ColorBlock cb = btnImage.colors;
        cb.highlightedColor = new Color(0.85f, 0.3f, 0.3f); //red
        cb.pressedColor = new Color(0, 0, 0); //black

        btnImage.colors = cb;
        btnImage.onClick.AddListener(() => OnClickImage(idx)); 
    }

    //assign a mesh to each obj
    public void AddMeshToObj(int idx)
    {
        //add meshfilters/renderers to gameobjects that will hold the meshes
        MeshFilter mf = _objs[idx].AddComponent<MeshFilter>();
        _meshFilters[idx] = mf;
        mf.gameObject.AddComponent<MeshRenderer>();
        mf.gameObject.GetComponent<Renderer>().material = _material;

    }

    //adds a pointlight for each object
    //could do with improving the distance from object based on scale
    public void AddLight(int idx)
    {
        GameObject cam = GameObject.Find("cam" + idx);
        GameObject lightGameObject = new GameObject("light" + idx);
        Light light = lightGameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = 10;

        lightGameObject.transform.SetParent(_cams[idx].GetComponent<Transform>(), true);
        light.transform.localPosition = new Vector3(-1.5f, 0.5f, 0.25f);

        _lights[idx] = lightGameObject;
    }

    //adds a disabled rotation script to each object
    public void AddRotationScript(int idx)
    {
        _rObjs[idx].AddComponent<ObjectControl>();
        _rObjs[idx].GetComponent<ObjectControl>().enabled = false;
    }

    #endregion GUI initialisation scripts

    //applies meshes to mfs
    //need to resolve some issues with infinitely large meshes/submeshes occasionally
    public void MeshesToMeshFilters(Mesh[] _meshes)
    {
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshFilters[i].mesh = _meshes[i];
            Vector3 bounds = _meshes[i].bounds.size;
            float maxBound = Mathf.Max(bounds[0], bounds[1], bounds[2] / 2); //weight the z axis down a bit, as plane of view is x y
            maxBound = maxBound < 0.2 ? 0.2f : maxBound; //stops infinitely large scaling up

            if (float.IsInfinity(maxBound)) { Debug.Log("Issue with infinitely large dimensions, Mesh won't render"); }

            _meshFilters[i].transform.localScale = (1 / maxBound) * Vector3.one;
            _objs[i].transform.localPosition = -_meshes[i].bounds.center / maxBound;
        }
    }

    //applies partially complete mesh to mf
    public void MeshRedraw(Mesh stepMesh)
    {
        _activeObj.GetComponentInChildren<MeshFilter>().mesh = stepMesh;
    }

    //writes turtle instructions to text object for animation

    public void InstructionsRewrite(string partialInstructions)
    {
        GameObject.Find("TextPartial").GetComponent<Text>().text = partialInstructions;
    }

    //gets the number of the currently zoomed in object if so
    public int GetActiveNumber()
    {
        return int.Parse(_activeObj.name.Substring(4));
    }

    //updates the GUI text boxes when evolve is clicked
    public void UpdateGuiText(RuleSet[] ruleSets, int generation)
    {
        for (int i = 0; i < _childCount; i++)
        {
            _captions[i].text = "Axiom : " + ruleSets[i]._axiom + "\n";

            var keyArray = ruleSets[i]._rules.Keys.ToArray();
            string ruleNames = string.Join("", keyArray);

            for (int j = 0; j < ruleSets[i]._ruleCount; j++)
            {
                _captions[i].text += ruleNames[j] + " -> " + ruleSets[i]._rules[ruleNames[j].ToString()] + "\n";
            }
        }

        Text textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
        textGeneration.text = "GENERATION : " + generation.ToString();
        _inputSelection.text = "";
    }

    //adds selection if user clicks on image
    public void OnClickImage(int objNum)
    {
        if (_inputSelection.text == "") { _inputSelection.text = objNum.ToString(); }
        else { _inputSelection.text = _inputSelection.text + " " + objNum; }
        string[] inputs = _inputSelection.text.Split(' ').Reverse().Take(5).Reverse().ToArray(); //take the last 5 inputs
        _inputSelection.text = string.Join(" ", inputs);
    }

    //zooms in on object if users clicks caption
    public void OnClickFocus(int objNum)
    {
        _zoomed = true;
        _activeCam = _cams[objNum];
        _activeObj = _rObjs[objNum];

        _mainCam.gameObject.SetActive(false);

        _activeRenderTexture = _activeCam.gameObject.GetComponent<Camera>().targetTexture;

        _activeCam.gameObject.GetComponent<Camera>().targetTexture = null;

        _canvasInfo.SetParent(_activeCam.transform, true);
        _canvasInfo.gameObject.SetActive(true); //turn on the info HUD

        Text textInfo = GameObject.Find("TextObjectInfo").GetComponent<Text>();
        textInfo.text = _captions[objNum].text.ToUpper();

        toggleZoomedScript(); //turns on the object control script
    }

    public void toggleZoomedScript()
    {
        ObjectControl objectScript = _activeObj.GetComponent<ObjectControl>();
        objectScript.enabled = !objectScript.enabled;
    }

    //determines right click action
    public void OnClickRightClick()
    {
        if (_zoomed)
        {
            OnClickZoomOut();  //zoom out if zoomed in
        }
        else
        {
            _inputSelection.text = ""; //clear inputs if on main screen
        }
    }

    //zooms out from object to main gui
    public void OnClickZoomOut()
    {
        _mainCam.gameObject.SetActive(true);
        _activeCam.gameObject.GetComponent<Camera>().targetTexture = _activeRenderTexture;
        _zoomed = false;
        _canvasInfo.gameObject.SetActive(false); //turn off the info HUD
        toggleZoomedScript();
    }

}



//Notes for improvements
//Active object could be more elegant, using substring not ideal

