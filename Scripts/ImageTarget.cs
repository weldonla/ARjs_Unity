using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImageTarget : MonoBehaviour
{
    [HideInInspector]
    public string patternName = "default";
    [HideInInspector]
    public string destination = "Assets/AR.js-master/aframe/";
    //@todo remove when you can implement nft images
    [HideInInspector]
    public bool isNftImage = false;
    public bool smooth = true;
    public int smoothCount = 10;
    public float smoothTolerance = 0.01f;
    public int smoothThreshold = 5;

    private void OnValidate()
    {
        destination = "Assets/AR.js-master/aframe/" + SceneManager.GetActiveScene().name + "/";
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
            AssetDatabase.Refresh();
        }
    }

}

[CustomEditor(typeof(ImageTarget))]
public class ImageTargetEditor: Editor 
{
    ImageTarget t;
    SerializedObject GetTarget;
    void OnEnable()
    {
        t = (ImageTarget)target;
        GetTarget = new SerializedObject(t);
    }
    public override void OnInspectorGUI()
    {
        t.isNftImage = GUILayout.Toggle(t.isNftImage, "isNftImage");
        GetTarget.ApplyModifiedProperties();
        if(t.isNftImage) {
            t.smooth = GUILayout.Toggle(t.smooth, "Smooth");
            if(t.smooth) {
                t.smoothCount = EditorGUILayout.IntField("Smooth Count", t.smoothCount);
                t.smoothTolerance = EditorGUILayout.FloatField("Smooth Tolerance", t.smoothTolerance);
                t.smoothThreshold = EditorGUILayout.IntField("Smooth Threshold", t.smoothThreshold);
            }
        }
    }
}
