using UnityEngine;
using UnityEngine.UI;
using System.Linq;
class View
{
    MeshFilter[] _meshFilters;
    int _childCount;
    InputField inputSelection;

    public View(int childCount)
    {
        _childCount = childCount;
        _meshFilters = new MeshFilter[_childCount];
        inputSelection = GameObject.Find("InputSelection").GetComponent<InputField>();

        for (int i = 0; i < _childCount; i++)
        {
            AddGuiItem(i);
        }
    }

    public void MeshesToMeshFilters(Mesh[] meshes)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            _meshFilters[i].mesh = meshes[i];
            Vector3 bounds = meshes[i].bounds.size;
            float maxBound = Mathf.Max(bounds[0], bounds[1], bounds[2]);
            _meshFilters[i].transform.localScale = Vector3.one / (maxBound);
        }
    }

    public void UpdateGuiText(RuleSet[] ruleSets, int generation)
    {
        for (int i = 0; i < _childCount; i++)
        {
            Text textCaption = GameObject.Find("TextCaption" + i.ToString()).GetComponent<Text>();
            textCaption.text = "F -> " + ruleSets[i]._rules["F"] + "\n" + "G -> " + ruleSets[i]._rules["G"];
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
        rawImages[idx].gameObject.GetComponent<Button>().onClick.AddListener(() => OnClickImage(imageName));

        //Add game objects to hold text captions
        GameObject textObject = new GameObject();
        textObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform);

        //Add text to game objects created and format/position
        Text textCaption = textObject.AddComponent<Text>();
        textCaption.name = "TextCaption" + idx.ToString();
        textCaption.font = Resources.Load<Font>("Fonts/UnicaOne-Regular");
        textCaption.fontSize = 10;
        textCaption.horizontalOverflow = HorizontalWrapMode.Overflow;
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f);
        textCaption.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35f);
        textCaption.rectTransform.localScale = new Vector3(1, 1, 1);
        textCaption.rectTransform.localPosition = rawImages[idx].rectTransform.localPosition + new Vector3(0, -90, 0);

        //add meshfilters/renderers to gameobjects that will hold the meshes
        MeshFilter mf = GameObject.Find("obj" + idx.ToString()).AddComponent<MeshFilter>();
        _meshFilters[idx] = mf;
        mf.gameObject.AddComponent<MeshRenderer>();
        mf.gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default")); ;
    }
    public void OnClickImage(string rawSelectionNum)
    {
        if (inputSelection.text == "") { inputSelection.text = rawSelectionNum.Substring(3).ToString(); }
        else {inputSelection.text = inputSelection.text + " " + rawSelectionNum.Substring(3).ToString(); }
        string[] inputs = inputSelection.text.Split(' ').Reverse().Take(5).Reverse().ToArray(); //take the last 5 inputs
        inputSelection.text = string.Join(" ", inputs);
    }
}

