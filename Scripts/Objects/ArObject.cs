using System.Text;
using System.Reflection;
using UnityEngine;
public abstract class ArObject {
    public string _src { get; set; }
    public string _id { get; set; }
    public string _class { get; set; } 
    public string _position { get; set; }
    public string _rotation { get; set; } 
    public string _color { get; set; }
    public string _transparent { get; set; }

    public abstract string getHtmlString(KeyFrameList keyList);

    public abstract void addKeyFrames(KeyFrameList keyList);

    public string getObjectPropertiesString() {
        StringBuilder sb = new StringBuilder();
        System.Type type = this.GetType();
        PropertyInfo[] properties = type.GetProperties();
        
        foreach (PropertyInfo property in properties)
        {
            sb.Append($"{property.Name.Trim('_')}={property.GetValue(this)} ");
            // Debug.Log($"{property.Name.Trim('_')}={property.GetValue(this)} ");
        }
        return sb.ToString();
    }
}