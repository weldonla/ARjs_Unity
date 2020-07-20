using System.Text;
using System.Reflection;
using UnityEngine;
public class Video : ArObject {
    public string _width;
    public string _height;

    private float x_pos_offset = -1/10;
    private float y_pos_offset = 1/10;
    private float z_pos_offset = 1/10;
    private float x_rot_offset = 1;
    private float y_rot_offset = -1;
    private float z_rot_offset = -1;

    public void initialize(GameObject childToAdd, string textureName) {
        Transform transform = childToAdd.transform;

        _width = $"{transform.localScale.x}";
        _height = $"{transform.localScale.y}";
        _position = $"{transform.localPosition.x*x_pos_offset} {transform.localPosition.y*y_pos_offset} {transform.localPosition.z*z_pos_offset}";
        _rotation = $"{transform.localEulerAngles.x*x_rot_offset} {transform.localEulerAngles.y*y_rot_offset} {transform.localEulerAngles.z*z_rot_offset}";
        _color = "#" + ColorUtility.ToHtmlStringRGB(childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.color);
        _src = textureName != null ? "textures/" + textureName + ".png" : "";
    }

    public override string getHtmlString(KeyFrameList keyList) {
        StringBuilder sb = new StringBuilder();
        return "";
    }
    public override void addKeyFrames(KeyFrameList keyList) {
        
    }
}