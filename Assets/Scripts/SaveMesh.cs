using UnityEngine;
using UnityEditor;

// Usage: Attach to gameobject, assign target gameobject (from where the mesh is taken), Run, Press savekey

public class SaveMesh : MonoBehaviour
{

    public KeyCode saveKey = KeyCode.A;
    public string saveName = "SavedMesh";
    public Transform selectedGameObject;

    void Update()
    {
        
        if (Input.GetKeyDown(saveKey))
        {
            Debug.Log("test");
            SaveAsset();
        }
    }

    void SaveAsset()
    {
        var mf = GameObject.Find("obj0").GetComponent<MeshFilter>();

            var savePath = "Assets/" + saveName + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            AssetDatabase.CreateAsset(mf.mesh, savePath);
        
    }
}
