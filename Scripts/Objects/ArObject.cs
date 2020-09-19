using System.Text;
using System.Reflection;
using UnityEngine;
using System;
public abstract class ArObject {
    // General Purpose
    public string HtmlTag;
    public GameObject childToAdd;
    public string objectId;
    public string scaleX_name;
    public string scaleY_name;
    public string scaleZ_name;

    // HTML Properties
    public string _src { get; set; }
    public string _id { get; set; }
    public string _class { get; set; } 
    public string _position { get; set; }
    public string _rotation { get; set; } 
    public string _color { get; set; }
    public string _transparent { get; set; }

    // Transform Offsets
    public float x_pos_offset;
    public float y_pos_offset;
    public float z_pos_offset;
    public float x_rot_offset;
    public float y_rot_offset;
    public float z_rot_offset;
    public float x_scale_offset;
    public float y_scale_offset;
    public float z_scale_offset;

    public virtual void setPropertyValues(GameObject childToAdd, string textureName, string id) {
        Transform transform = childToAdd.transform;

        this.childToAdd = childToAdd;
        objectId = id;

        _class = "intersectable";
        _position = $"{transform.localPosition.x*x_pos_offset} {transform.localPosition.y*y_pos_offset} {transform.localPosition.z*z_pos_offset}";
        _rotation = $"{transform.localEulerAngles.x*x_rot_offset} {transform.localEulerAngles.y*y_rot_offset} {transform.localEulerAngles.z*z_rot_offset}";
        _color = "#" + ColorUtility.ToHtmlStringRGB(childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.color);
        _src = textureName != null ? "textures/" + textureName + ".png" : null;

        // Set Scale properties, which vary depending on object type
        System.Type type = this.GetType();
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            string propertyPrefix = property.Name.Split('_')[0];
            string propertyName = property.Name.Split('_')[1];

            switch(propertyPrefix) {
                case "scaleX" :
                    property.SetValue(this, $"{transform.localScale.x*x_scale_offset}");
                    scaleX_name = propertyName;
                    break;
                case "scaleY" :
                    property.SetValue(this, $"{transform.localScale.y*y_scale_offset}");
                    scaleY_name = propertyName;
                    break;
                case "scaleZ" :
                    property.SetValue(this, $"{transform.localScale.z*z_scale_offset}");
                    scaleZ_name = propertyName;
                    break;
            }
        }
    }

    public string getHtmlString(KeyFrameList keyList) {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"<{HtmlTag} {getObjectPropertiesString()}");
        if(keyList != null) sb.AppendLine($"{getKeyFramesString(keyList)}");
        sb.Append($"></{HtmlTag}>");
        return sb.ToString();
    }

    public virtual string getKeyFramesString(KeyFrameList keyList) {
        StringBuilder sb = new StringBuilder();
        foreach (WeldonKeyFrame frame in keyList.frameList)
        {
            int index = keyList.frameList.FindIndex(obj => obj == frame);
            string loopTrueString = "";
            string animTrigger = "";
            string posFrom = "", rotFrom = "", scaleXFrom = "", scaleYFrom = "", scaleZFrom = "";
            WeldonKeyFrame prevFrame = new WeldonKeyFrame();
            if (index > 0)
            {
                prevFrame = keyList.frameList[index - 1];
                posFrom = $"from: {-prevFrame.posX*x_pos_offset} {prevFrame.posY*y_pos_offset} {prevFrame.posZ*z_pos_offset};";
                rotFrom = $"from: {prevFrame.rotX*x_rot_offset} {prevFrame.rotY*y_rot_offset} {prevFrame.rotZ*z_rot_offset};";
                scaleXFrom = $"from: {prevFrame.scalX*x_scale_offset};";
                scaleYFrom = $"from: {prevFrame.scalY*y_scale_offset};";
                scaleZFrom = $"from: {prevFrame.scalZ*z_scale_offset};";

                animTrigger = $"startEvents: animationcomplete__{objectId}_f{index-1}" + ((index==1 && childToAdd.GetComponent<AnimationHelper>().loop)? $", animationcomplete__{objectId}_f{keyList.frameList.Count-1};" : ";");
            }
            else
            {
                if (childToAdd.GetComponent<AnimationHelper>().onClick) animTrigger = $"startEvents: mousedown;";
            }

            string posTo = $"to: {frame.posX*x_pos_offset} {frame.posY*y_pos_offset} {frame.posZ*z_pos_offset};",
                rotTo = $"to: {frame.rotX*x_rot_offset} {frame.rotY*y_rot_offset} {frame.rotZ*z_rot_offset};",
                scaleXTo = $"to: {frame.scalX*x_scale_offset};",
                scaleYTo = $"to: {frame.scalY*y_scale_offset};",
                scaleZTo = $"to: {frame.scalZ*z_scale_offset};";

            //if (childToAdd.GetComponent<AnimationHelper>().loop) loopTrueString = $"repeat = \"indefinite\"";
            bool isFirstFrame = prevFrame.time.Equals(-1) ? true : false;
            if (isFirstFrame) prevFrame.time = 0;
            if (frame.IsDifferentPosition(prevFrame) || isFirstFrame) sb.AppendLine($"animation__{objectId}_f{index}=\" property: position; {posFrom} {posTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if (frame.IsDifferentRotation(prevFrame) || isFirstFrame) sb.AppendLine($"animation__{objectId}_f{index}=\" property: rotation; {rotFrom} {rotTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if ((frame.IsDifferentWidth(prevFrame) || isFirstFrame) && scaleX_name != null) sb.AppendLine($"animation__{objectId}_f{index}=\" property: {scaleX_name}; {scaleXFrom} {scaleXTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if ((frame.IsDifferentHeight(prevFrame) || isFirstFrame) && scaleY_name != null) sb.AppendLine($"animation__{objectId}_f{index}=\" property: {scaleY_name}; {scaleYFrom} {scaleYTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
            if ((frame.IsDifferentHeight(prevFrame) || isFirstFrame) && scaleZ_name != null) sb.AppendLine($"animation__{objectId}_f{index}=\" property: {scaleZ_name}; {scaleZFrom} {scaleZTo} dur: {(frame.time - prevFrame.time) * 1000}; easing: linear; {animTrigger}\"");
        }
        return sb.ToString();
    }

    public string getObjectPropertiesString() {
        StringBuilder sb = new StringBuilder();
        System.Type type = this.GetType();
        PropertyInfo[] properties = type.GetProperties();
        
        foreach (PropertyInfo property in properties)
        {
            if(property.GetValue(this) != null){
                var propertyArray = property.Name.Split('_');
                propertyArray[0] = "";
                sb.Append($"{String.Join("-", propertyArray).Remove(0,1)}=\"{property.GetValue(this)}\" ");
                // Debug.Log($"{property.Name.Split('_')[0]}={property.GetValue(this)} ");
            }
        }
        return sb.ToString();
    }
}