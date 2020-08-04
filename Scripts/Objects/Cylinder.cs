public class Cylinder : ArObject {
    public string scaleX_radius { get; set; }
    public string scaleY_height { get; set; }

    public void initialize() {
        HtmlTag = TagNames.cylinder;

        x_pos_offset = -1f/10f;
        y_pos_offset = 1f/10f;
        z_pos_offset = 1f/10f;
        x_rot_offset = 1f;
        y_rot_offset = -1f;
        z_rot_offset = -1f;
        x_scale_offset = 1f/20f;
        y_scale_offset = 1f/5f;
        z_scale_offset = 1f/20f;
    }
}