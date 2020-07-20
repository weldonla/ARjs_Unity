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
    public string scaleX_width { get; set; }
    public string scaleY_height { get; set; }

    public void initialize() {
        HtmlTag = TagNames.plane;

        x_pos_offset = -1f/10f;
        y_pos_offset = 1f/10f;
        z_pos_offset = 1f/10f;
        x_rot_offset = 1f;
        y_rot_offset = -1f;
        z_rot_offset = -1f;
        x_scale_offset = 1f;
        y_scale_offset = 1f;
        z_scale_offset = 1f;
    }
}