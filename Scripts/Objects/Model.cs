using System.Text;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;
public class Model : ArObject {
    // HTML properties
    public string _obj_model {get; set;}
    public string scaleX_width { get; set; }
    public string scaleY_height { get; set; }
    public string scaleZ_depth { get; set; }

    public void initialize() {
        HtmlTag = TagNames.model;

        x_pos_offset = -1f/10f;
        y_pos_offset = 1f/10f;
        z_pos_offset = 1f/10f;
        x_rot_offset = 1f;
        y_rot_offset = -1f;
        z_rot_offset = -1f;
        x_scale_offset = 1f/10f;
        y_scale_offset = 1f/10f;
        z_scale_offset = 1f/10f;
    }

    public void setObjectModelString(int index) {
        _obj_model=$"obj: #{childToAdd.name + "_Asset_obj_" + index}; mtl: #{childToAdd.name + "_Asset_mtl_" + index}";
    }

    public string getAssetString(string folderPath, int i) {
        StringBuilder sb = new StringBuilder();
        if (!Directory.Exists(folderPath + "models/")) Directory.CreateDirectory(folderPath + "models/");
        CustomModelHelper modelHelper = childToAdd.GetComponent<CustomModelHelper>();
        if (File.Exists(folderPath + "models/" + modelHelper.objName))
        {
            sb.AppendLine($"<a-asset-item id=\"{childToAdd.name + "_Asset_obj_" + i}\" src=\"models/{modelHelper.objName}\"></a-asset-item>");
        }
        else
        {
            Debug.LogError($"The model file {modelHelper.objName} doesn't seem to exist in the proper location.");
        }
        if (File.Exists(folderPath + "models/" + modelHelper.mtlName))
        {
            sb.AppendLine($"<a-asset-item id=\"{childToAdd.name + "_Asset_mtl_" + i}\" src=\"models/{modelHelper.mtlName}\"></a-asset-item>");
        }
        else
        {
            Debug.LogWarning("The object doesn't have a material.");
        }
        return sb.ToString();
    }
}