using UnityEngine;
using UnityEngine.UI;
using System.Linq;
class View
{
    MeshFilter[] _meshFilters;
    int _childCount;
    InputField inputSelection;
    Camera _main, _activeCam;
    RenderTexture _temp;
    bool _zoomStatus;
    Material _material;

    public View(int childCount, Material material)
    {
        _childCount = childCount;
        _meshFilters = new MeshFilter[_childCount];
        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();
        _main = GameObject.Find("Main Camera").GetComponent<Camera>();
        _zoomStatus = false;
        _material = material;

        for (int i = 0; i < _childCount; i++)
        {
            AddGuiItem(i);
            AddLights(i);
        }
    }

    public void MeshesToMeshFilters(Mesh[] meshes)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            _meshFilters[i].mesh = meshes[i];
            Vector3 bounds = meshes[i].bounds.size;
            float maxBound = Mathf.Max(bounds[0], bounds[1], bounds[2]); //leaving out the z axis for the moment
            Debug.Log(i + " " + maxBound);

            if(maxBound != 0)
                _meshFilters[i].transform.localScale = (1/maxBound) * _meshFilters[i].transform.localScale;

            //the bounds are changing everytime.
        }
    }

    public void UpdateGuiText(RuleSet[] ruleSets, int generation)
    {
        for (int i = 0; i < _childCount; i++)
        {
            Text textCaption = GameObject.Find("TextCaption" + i.ToString()).GetComponent<Text>();
            textCaption.text = "Axiom : " + ruleSets[i]._axiom + "\n";
            string ruleNames = "FGH";

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
        RawImage[] rawImages = GameObject.Find("Canvas").GetComponentsInChildren<RawImage>();
        //Add button to each image
        rawImages[idx].gameObject.AddComponent<Button>();
        string imageName = rawImages[idx].gameObject.name;
        string imageNum = imageName.Substring(3).ToString();
        rawImages[idx].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageNum));
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
        textCaption.rectTransform.localPosition = rawImages[idx].rectTransform.localPosition + new Vector3(0, -90, 0);

        textCaption.gameObject.AddComponent<Button>();
        textCaption.gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickFocus(imageNum));

        //add meshfilters/renderers to gameobjects that will hold the meshes
        MeshFilter mf = GameObject.Find("obj" + idx.ToString()).AddComponent<MeshFilter>();
        _meshFilters[idx] = mf;
        mf.gameObject.AddComponent<MeshRenderer>();
        mf.gameObject.GetComponent<Renderer>().material = _material;
    }

    //adds a pointlight for each object
    public void AddLights(int idx)
    {
        GameObject obj = GameObject.Find("obj" + idx);
        GameObject lightGameObject = new GameObject("light" + idx);
        Light light = lightGameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = 3;
        Debug.Log(obj.transform.localScale);
        lightGameObject.transform.SetParent(obj.GetComponent<RectTransform>(), false);


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
    }

    public void OnClickZoomOut()
    {
        if (_zoomStatus)
        {
            _main.gameObject.SetActive(true);
            _activeCam.gameObject.GetComponent<Camera>().targetTexture = _temp;
            _zoomStatus = false;
        }
        else
        {
            Debug.Log("Right Click zooms out only when zoomed in");

        }
    }
}

