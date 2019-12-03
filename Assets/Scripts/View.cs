using UnityEngine;
using UnityEngine.UI;
using System.Linq;
class View
{
    MeshFilter[] _meshFilters;
    RawImage[] _rawImages;
    GameObject[] _lights;
    int _childCount;
    InputField inputSelection;
    Camera _main, _activeCam;
    RenderTexture _temp;
    bool _zoomStatus;
    Material _material;
    RectTransform _canvasInfo;

    public View(int childCount, Material material)
    {
        _childCount = childCount;
        _meshFilters = new MeshFilter[_childCount];
        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _zoomStatus = false;
        _material = material;
        _rawImages = GameObject.Find("Canvas").GetComponentsInChildren<RawImage>();
        _canvasInfo = GameObject.Find("CanvasInfo").GetComponent<RectTransform>();
        _lights = new GameObject[_childCount];

        for (int i = 0; i < _childCount; i++)
        {
            AddGuiItem(i);
            AddLight(i);
            AddRotationScript(i);
        }
    }

    public void MeshesToMeshFilters(Mesh[] meshes)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            _meshFilters[i].mesh = meshes[i];
            Vector3 bounds = meshes[i].bounds.size;
            float maxBound = Mathf.Max(bounds[0], bounds[1], bounds[2]); //leaving out the z axis for the moment

            _meshFilters[i].transform.localPosition = GameObject.Find("cam" + i.ToString()).GetComponent<Camera>().transform.localPosition + new Vector3(0, 0, 1);
            //_meshFilters[i].transform.localPosition = _meshFilters[i].transform.parent.transform.localPosition;
            //GameObject.Find("obj" + i.ToString()).GetComponent<Transform>();
            Debug.Log("max bound " + maxBound);
            if (maxBound != 0)
            { _meshFilters[i].transform.localScale = (1 / maxBound) * Vector3.one;
              _lights[i].GetComponent<Light>().intensity = 1.5f / maxBound;
            }
        }
    }

    public void UpdateGuiText(RuleSet[] ruleSets, int generation)
    {
        for (int i = 0; i < _childCount; i++)
        {
            Text textCaption = GameObject.Find("TextCaption" + i.ToString()).GetComponent<Text>();


            textCaption.text = "Axiom : " + ruleSets[i]._axiom + "\n";

            var keyArray = ruleSets[i]._rules.Keys.ToArray();
            string ruleNames = string.Join("", keyArray);

            for (int j = 0; j < ruleSets[i]._ruleCount; j++)
            {
                textCaption.text += ruleNames[j] + " -> " + ruleSets[i]._rules[ruleNames[j].ToString()] + "\n";
            }
        }

        Text textGeneration = GameObject.Find("TextGeneration").GetComponent<Text>();
        textGeneration.text = "GENERATION : " + generation.ToString();
        inputSelection.text = "";
    }

    //adds the text captions and buttons to raw images
    public void AddGuiItem(int idx)
    {
        //Add button to each image
        _rawImages[idx].gameObject.AddComponent<Button>();
        string imageName = _rawImages[idx].gameObject.name;
        string imageNum = imageName.Substring(3).ToString();
        _rawImages[idx].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageNum));
        //Add game objects to hold text captions
        GameObject textObject = new GameObject();
        textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

        //Add text to game objects created and format/position
        Text textCaption = textObject.AddComponent<Text>();
        textCaption.name = "TextCaption" + idx.ToString();
        textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
        textCaption.fontSize = 8;
        textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40f);
        textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
        textCaption.rectTransform.localPosition = _rawImages[idx].rectTransform.localPosition + new Vector3(0, -90, 0);

        textCaption.gameObject.AddComponent<Button>();
        textCaption.gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickFocus(imageNum));

        //add meshfilters/renderers to gameobjects that will hold the meshes
        MeshFilter mf = GameObject.Find("obj" + idx.ToString()).AddComponent<MeshFilter>();
        _meshFilters[idx] = mf;
        mf.gameObject.AddComponent<MeshRenderer>();
        mf.gameObject.GetComponent<Renderer>().material = _material;
    }

    //adds a pointlight for each object
    public void AddLight(int idx)
    {
        
        GameObject cam = GameObject.Find("cam" + idx);
        GameObject lightGameObject = new GameObject("light" + idx);
        Light light = lightGameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = 2;
 
        lightGameObject.transform.SetParent(cam.GetComponent<Transform>(), false);
        _lights[idx] = lightGameObject;
    }

    public void AddRotationScript(int idx)
    {
        GameObject obj = GameObject.Find("obj" + idx);
        obj.AddComponent<ObjectRotate>();
     //   obj.GetComponent<ObjectRotate>().enabled = false;
    }

    public void OnClickImage(string rawSelectionNum)
    {
        if (inputSelection.text == "") { inputSelection.text = rawSelectionNum; }
        else {inputSelection.text = inputSelection.text + " " + rawSelectionNum; }
        string[] inputs = inputSelection.text.Split(' ').Reverse().Take(5).Reverse().ToArray(); //take the last 5 inputs
        inputSelection.text = string.Join(" ", inputs);

    }

    public void OnClickFocus(string rawSelectionNum)
    {
        _zoomStatus = true;
        Camera objectCam = GameObject.Find("cam" + rawSelectionNum).GetComponent<Camera>();
        _main.gameObject.SetActive(false);
        _temp = objectCam.gameObject.GetComponent<Camera>().targetTexture;
        _activeCam = objectCam;
        objectCam.gameObject.GetComponent<Camera>().targetTexture = null;


        _canvasInfo.SetParent(objectCam.transform, true);
        _canvasInfo.gameObject.SetActive(true);

        Text textInfo = GameObject.Find("TextObjectInfo").GetComponent<Text>();
        Text textCaption = GameObject.Find("TextCaption" + rawSelectionNum).GetComponent<Text>();
        textInfo.text = textCaption.text.ToUpper();


       // ObjectRotate objectScript = GameObject.Find("obj" + rawSelectionNum).GetComponent<ObjectRotate>();
       // objectScript.gameObject.SetActive(true);
    }

    public void OnClickRightClick()
    {
        if (_zoomStatus)
        {
            //zoom out if zoomed in
            OnClickZoomOut();
        }
        else
        {
            //clear inputs if on main screen
            inputSelection.text = "";
        }
    }
    public void OnClickZoomOut()
    {
        _main.gameObject.SetActive(true);
        _activeCam.gameObject.GetComponent<Camera>().targetTexture = _temp;
        _zoomStatus = false;
        _canvasInfo.gameObject.SetActive(false);
    }
}

