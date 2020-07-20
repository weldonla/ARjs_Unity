using System.Text;
using System.Reflection;
using UnityEngine;
public class Plane : ArObject {
    // var Plane = (width: childToAdd.transform.localScale.x, height: childToAdd.transform.localScale.y,
    //                             position: -childToAdd.transform.localPosition.x / 10 + " " + childToAdd.transform.localPosition.y / 10 + " " + childToAdd.transform.localPosition.z / 10,
    //                             rotation: childToAdd.transform.localEulerAngles.x + " " + -childToAdd.transform.localEulerAngles.y + " " + -childToAdd.transform.localEulerAngles.z,
    //                             color: "#" + ColorUtility.ToHtmlStringRGB(childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.color),
    //                             src: textureName != null ? "textures/" + textureName + ".png" : "");
    // sb.AppendLine($"<a-plane src=\"{Plane.src}\" id=\"{childToAdd.name + "_" + i}\" class=\"intersectable\" width=\"{Plane.width}\" height=\"{Plane.height}\" position=\"{Plane.position}\" rotation=\"{Plane.rotation}\" color=\"{Plane.color}\" transparent={transparency}");
    public string _width { get; set; }
    public string _height { get; set; }

    private float x_pos_offset = -1f/10f;
    private float y_pos_offset = 1f/10f;
    private float z_pos_offset = 1f/10f;
    private float x_rot_offset = 1f;
    private float y_rot_offset = -1f;
    private float z_rot_offset = -1f;
    

    public void initialize(GameObject childToAdd, string textureName) {
        Transform transform = childToAdd.transform;

        Debug.Log(transform.localPosition.y);
        Debug.Log(y_pos_offset);
        Debug.Log(transform.localPosition.y*y_pos_offset);

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