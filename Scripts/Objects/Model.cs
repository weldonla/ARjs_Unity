using System.Text;
using System.Reflection;
using System;
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
}