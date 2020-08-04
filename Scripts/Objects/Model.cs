using System.Text;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;
public class Model : ArObject {
    // HTML properties
    public string _obj_model {get; set;}
    public string _scale { get; set; }

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

    public override void setPropertyValues(GameObject childToAdd, string textureName, string id) {
        Transform transform = childToAdd.transform;

        this.childToAdd = childToAdd;
        objectId = id;

        _class = "intersectable";
        _position = $"{transform.localPosition.x*x_pos_offset} {transform.localPosition.y*y_pos_offset} {transform.localPosition.z*z_pos_offset}";
        _rotation = $"{transform.localEulerAngles.x*x_rot_offset} {transform.localEulerAngles.y*y_rot_offset} {transform.localEulerAngles.z*z_rot_offset}";
        _scale = $"{transform.localScale.x*x_scale_offset} {transform.localScale.y*y_scale_offset} {transform.localScale.z*z_scale_offset}";
        _color = "#" + ColorUtility.ToHtmlStringRGB(childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.color);
        _src = textureName != null ? "textures/" + textureName + ".png" : null;
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

    public override string getKeyFramesString(KeyFrameList keyList) {
        StringBuilder sb = new StringBuilder();
        foreach (WeldonKeyFrame frame in keyList.frameList)
        {
            int index = keyList.frameList.FindIndex(obj => obj == frame);
            string loopTrueString = "";
            string animTrigger = "";
            string posFrom = "", rotFrom = "", scaleFrom = "";
            WeldonKeyFrame prevFrame = new WeldonKeyFrame();
            if (index > 0)
            {
                prevFrame = keyList.frameList[index - 1];
                posFrom = $"from: {prevFrame.posX*z_pos_offset} {prevFrame.posY*y_pos_offset} {prevFrame.posZ*z_pos_offset};";
                rotFrom = $"from: {prevFrame.rotX*x_rot_offset} {prevFrame.rotY*y_rot_offset} {prevFrame.rotZ*z_rot_offset};";
                scaleFrom = $"from: {prevFrame.scalX*x_scale_offset} {prevFrame.scalY*y_scale_offset} {prevFrame.scalZ*y_scale_offset};";

                animTrigger = $"startEvents: animationcomplete__{objectId}_f{index-1}" + ((index==1 && childToAdd.GetComponent<AnimationHelper>().loop)? $", animationcomplete__{objectId}_f{keyList.frameList.Count-1};" : ";");
            }
            else
            {
                if (childToAdd.GetComponent<AnimationHelper>().onClick) animTrigger = $"startEvents: mousedown;";
            }

            string posTo = $"to: {frame.posX*x_pos_offset} {frame.posY*y_pos_offset} {frame.posZ*z_pos_offset};",
                rotTo = $"to: {frame.rotX*x_rot_offset} {frame.rotY*y_rot_offset} {frame.rotZ*z_rot_offset};",
                scaleTo = $"to: {frame.scalX*x_scale_offset} {frame.scalY*y_scale_offset} {frame.scalZ*y_scale_offset};";

            //if (childToAdd.GetComponent<AnimationHelper>().loop) loopTrueString = $"repeat = \"indefinite\"";
            bool isFirstFrame = prevFrame.time.Equals(-1) ? true : false;
            if (isFirstFrame) prevFrame.time = 0;
            if (frame.IsDifferentPosition(prevFrame) || isFirstFrame) sb.AppendLine($"animation__{objectId}_f{index}=\" property: position; {posFrom} {posTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if (frame.IsDifferentRotation(prevFrame) || isFirstFrame) sb.AppendLine($"animation__{objectId}_f{index}=\" property: rotation; {rotFrom} {rotTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if ((frame.IsDifferentWidth(prevFrame) || isFirstFrame)) sb.AppendLine($"animation__{objectId}_f{index}=\" property: scale; {scaleFrom} {scaleTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
        }
        return sb.ToString();
    }
}