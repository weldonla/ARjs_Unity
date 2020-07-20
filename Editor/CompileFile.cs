using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CompileFile : MonoBehaviour
{
    [MenuItem("AR.js/Compile Files", true)]
    static bool CompileFileHTMLingvalidation()
    {
        if (GameObject.FindWithTag("ImageTarget") != null)
        {
            return true;
        }
        else return false;
    }


    //Creates a menu item for building out the objects under the ImageTarget in scene as HTML code and saves said code to a file.
    [MenuItem("AR.js/Compile Files", false, 17)]
    static void CompileFileHTML()
    {
        string folderPath = GameObject.FindWithTag("ImageTarget").GetComponent<ImageTarget>().destination;
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        if (!File.Exists("Assets/AR.js-master/aframe/fullscreen.png"))
        {
            File.Copy("Assets/ARjs_Unity/Icons/fullscreen.png", "Assets/AR.js-master/aframe/fullscreen.png");
        }
        if (!File.Exists("Assets/AR.js-master/aframe/exit_fullscreen.png"))
        {
            File.Copy("Assets/ARjs_Unity/Icons/exit_fullscreen.png", "Assets/AR.js-master/aframe/exit_fullscreen.png");
        }
        string fileName = "index.html";
        bool hasVideo = false;

        bool isNft = GameObject.FindWithTag("ImageTarget").GetComponent<ImageTarget>().isNftImage;

        StringBuilder sb = new StringBuilder();
        #region Top HTML
        sb.AppendLine("<!-- BEGIN: Top HTML -->");
        sb.Append(HtmlStrings.getTopHtml(isNft));
        sb.AppendLine("<!-- END: Top HTML -->");
        #endregion Top HTML

        Transform imageTarget = GameObject.FindGameObjectWithTag("ImageTarget").transform;
        if(imageTarget == null)
        {
            Debug.Log("AR.js error: There is no Image Target to Compile");
            return;
        }

        #region Unity Compiled Events
        //Adds in the actions of the children to javascript.
        sb.AppendLine("<!-- BEGIN: Unity Compiled Events -->");
        sb.AppendLine("<script>");

        //GLOBALS FOR ANIMATION UPDATE
        sb.AppendLine("var markerFound = 0;");
        bool hasBezier = false;
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            for(int j = 0; j<childToAdd.transform.childCount; j++)
            {
                if (childToAdd.transform.GetChild(j).name.ToLower().Contains("bezier"))
                {
                    hasBezier = true;
                }
            }
        }
        if (hasBezier)
        {
            //sb.AppendLine("var subtractTime = 0;");
            for (int i = 0; i < imageTarget.childCount; i++)
            {
                GameObject childToAdd = imageTarget.GetChild(i).gameObject;
                string childID = childToAdd.name + "_" + i;
                Bezier bez = childToAdd.GetComponentInChildren<Bezier>();
                if (bez != null)
                {
                    sb.AppendLine("var " + childID + "_CurrentPoint = 0;");
                    sb.AppendLine("var " + childID + "_SubtractTime = 0;");
                    sb.AppendLine("var " + childID + "_PointsArray = " + bez.PointsListString() + ";");
                }
            }
        }
        //END GLOBALS FOR ANIMATION UPDATE

        sb.AppendLine("AFRAME.registerComponent('button', {");
        sb.AppendLine("init: function () {");
        sb.AppendLine($"var elem = document.documentElement;");
        sb.AppendLine($"var marker = document.querySelector(\"#marker\");");
        sb.AppendLine($"var fullbutton = document.querySelector(\"#fullscreen\");");
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            if (childToAdd.tag == "Video")
            {

                hasVideo = true;
            }
        }
        if(hasVideo) sb.AppendLine($"var button = document.querySelector(\"#mutebutton\");");

        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;
            if (childToAdd.GetComponent<ButtonHelper>() != null)
            {
                sb.AppendLine($"var {id} = document.querySelector(\"#{id}\");");
            }

            if (childToAdd.tag == "Video")
            {
                hasVideo = true;
                sb.AppendLine($"var {id} = document.querySelector(\"#{childToAdd.name + "_Asset_" + i}\");");
            }
        }
        //marker event listeners
        sb.AppendLine("marker.addEventListener(\"markerFound\", function (evt) {");
        sb.AppendLine("markerFound = 1;");
        for(int i = 0; i<imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;

            if(childToAdd.tag == "Video")
            {
                string lineToAppend = id + ".play();";
                sb.AppendLine(lineToAppend);
            }
        }
        sb.AppendLine("});");
        sb.AppendLine("marker.addEventListener(\"markerLost\", function (evt) {");
        sb.AppendLine("markerFound = 0;");
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;

            if (childToAdd.tag == "Video")
            {
                string lineToAppend = id + ".pause();";
                sb.AppendLine(lineToAppend);
            }
        }
        sb.AppendLine("});");
        //end marker event listeners
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;

            if (childToAdd.GetComponent<ButtonHelper>() != null)
            {
                sb.AppendLine(@"open(""" + childToAdd.GetComponent<ButtonHelper>().URL + @""");");
                sb.AppendLine(id + @".addEventListener(""mousedown"", function(evt){");
                sb.AppendLine("});");
            }
            if (childToAdd.tag == "Video")
            {
                //string lineToAppend = "marker.addEventListener(\"markerFound\", function (evt) {\n" +
                	//id + ".play();\n" +
                	//"});\n" +
                	//"marker.addEventListener(\"markerLost\", function (evt) {\n" +
                	//id + ".pause();\n" +
                	//"});";
                
                string secondLine = "button.addEventListener(\"click\", function(evt){\n" +
                	"console.log(\"button clicked\")\n" +
                	"if(" + id + ".muted==true){\n" +
                	"button.innerHTML=\"Mute\";\n" +
                	id + ".muted=false;\n" +
                	"}else{\n" +
                	"button.innerHTML=\"Unmute\";\n" +
                	id + ".muted=true;\n" +
                	"}\n" +
                	"});";


                //sb.AppendLine(lineToAppend);
                sb.AppendLine(secondLine);
            }
        }
        //end marker event listeners

        sb.AppendLine(HtmlStrings.fullscreenButtonActionHTML);
        sb.AppendLine("},");
        //BEGIN: Tick function for bezier animations
        sb.AppendLine("tick: function (totalTime, deltaTime) {");
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;
            Bezier bez = childToAdd.GetComponentInChildren<Bezier>();
            if (bez != null)
            {
                sb.AppendLine($"var {id} = document.querySelector(\"#{id}\");");
                sb.AppendLine($"var {id}_Speed = {bez.speed/60};");
                sb.AppendLine($"var {id}_Time = (totalTime - {id}_SubtractTime) / 1000;");
            }
        }
        //sb.AppendLine("var time = (totalTime - subtractTime) / 1000;");
        sb.AppendLine("var dTime = deltaTime / 1000;");
        sb.AppendLine("");
        sb.AppendLine("if (markerFound == 1) {");
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;
            Bezier bez = childToAdd.GetComponentInChildren<Bezier>();
            if (bez != null)
            {
                sb.AppendLine($"{id}_Update();");
            }
        }
        sb.AppendLine("}");
        sb.AppendLine();
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            string id = childToAdd.name + "_" + i;
            Bezier bez = childToAdd.GetComponentInChildren<Bezier>();
            if (bez != null)
            {
                sb.AppendLine($"function {id}_Update()" + " {");
                sb.AppendLine($"var newPosition = bezierPath({id}_PointsArray[{id}_CurrentPoint], {id}_PointsArray[{id}_CurrentPoint + 1], { id}_PointsArray[{ id}_CurrentPoint + 2], { id}_PointsArray[{ id}_CurrentPoint + 3], {id}_Time *{ id}_Speed);");
                sb.AppendLine();
                sb.AppendLine($"if ({id}_Time*{id}_Speed>1) " + "{");
                sb.AppendLine($"{id}_CurrentPoint += 3;");
                sb.AppendLine($"{id}_SubtractTime = totalTime;");
                sb.AppendLine($"if ({id}_CurrentPoint >= {id}_PointsArray.length - 3) " + "{");
                sb.AppendLine($"{id}_CurrentPoint = 0;");
                sb.AppendLine("}");
                sb.AppendLine("}");
                sb.AppendLine();
                sb.AppendLine($"{id}.setAttribute('position', " + "{");
                sb.AppendLine("x: newPosition.x,");
                sb.AppendLine("y: newPosition.y,");
                sb.AppendLine("z: newPosition.z,");
                sb.AppendLine("});");
                sb.AppendLine("}");
            }
        }

        sb.AppendLine("function bezierEvaluate(p0, p1, p2, p3, t) {\n                " +
        	"var u = (1 - t);\n                " +
        	"var uu = u * u;\n                " +
        	"var uuu = u * u * u;\n                " +
        	"var tt = t * t;\n                " +
        	"var ttt = t * t * t;\n                " +
        	"//B(t) = (1-t)^3*P0 + 3*(1-t)^2*t*P1 + 3*(1-t)*t^2*P2 + t^3*P3 , 0 < t < 1\n                " +
        	"return (uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3);\n\n            " +
        	"}\n           " +
        	"function bezierPath(p0, p1, p2, p3, t) {\n                " +
        	"return new THREE.Vector3(\n                    " +
        	"bezierEvaluate(p0.x, p1.x, p2.x, p3.x, t),\n                    " +
        	"bezierEvaluate(p0.y, p1.y, p2.y, p3.y, t),\n                    " +
        	"bezierEvaluate(p0.z, p1.z, p2.z, p3.z, t)\n                " +
        	");\n            " +
        	"}\n\n        " +
        	"}");
        //END: Tick function for bezier animations
        sb.AppendLine("});");
        sb.AppendLine("</script>");
        sb.AppendLine("<!-- END: Unity Compiled Events -->");
        sb.AppendLine("");
        #endregion Unity Compiled Events

        //MiddleHTML
        sb.AppendLine("<!-- BEGIN: Middle HTML -->");
        sb.AppendLine(HtmlStrings.bodyHtml);
        if (hasVideo) sb.AppendLine(HtmlStrings.buttonHTML);
        sb.AppendLine(HtmlStrings.fullscreenButtonHTML);
        sb.AppendLine(HtmlStrings.middleHTML);
        sb.AppendLine("<!-- END: Middle HTML -->");
        sb.AppendLine("");

        #region Unity Compiled Assets
        sb.AppendLine("<!-- BEGIN: Unity Compiled Assets -->");
        sb.AppendLine("<a-assets>");
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            if(childToAdd.tag == "Video")
            {
                VideoPlayer player = childToAdd.GetComponentInChildren<VideoPlayer>();
                string location = player.clip.originalPath;
                string[] splitName = location.Split('/');
                string videoName = splitName[splitName.Length - 1];
                string destination = folderPath + "videos/" + videoName;
                if(!Directory.Exists(folderPath + "videos/")) Directory.CreateDirectory(folderPath + "videos/");
                if (File.Exists(destination)) File.Delete(destination);
                File.Copy(location, destination);
                sb.AppendLine($"<video id=\"{childToAdd.name + "_Asset_" + i}\" autoplay=\"false\" loop crossorigin=\"anonymous\" src=\"videos/{videoName}\" webkit-playsinline playsinline controls muted></video>");
            }

            if(childToAdd.tag == "Model")
            {
                string modelID = childToAdd.name.ToLower() + "_" + i;
                string textureName = childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture.name;
                Model newModel = new Model();
                newModel.initialize();
                newModel.setPropertyValues(childToAdd, textureName, modelID);
                newModel.setObjectModelString(i);
                sb.AppendLine(newModel.getAssetString(folderPath, i));
            }
        }
        sb.AppendLine("</a-assets>");
        sb.AppendLine("<!-- END: Unity Compiled Assets -->");
        #endregion Unity Compiled Assets

        sb.AppendLine("<!-- BEGIN: Add Image Target (marker) -->");
        sb.AppendLine(HtmlStrings.getTopMarkerHtml(isNft));
        sb.AppendLine("<!-- END: Add Image Target (marker) -->");
        sb.AppendLine("");

        #region Unity Compiled Objects
        sb.AppendLine("<!-- BEGIN: Unity Compiled Objects -->");
        //Adds in the physical object for each child of the ImageTarget
        for (int i = 0; i < imageTarget.childCount; i++)
        {
            
            GameObject childToAdd = imageTarget.GetChild(i).gameObject;
            Texture2D objectTexture = (Texture2D)childToAdd.GetComponentInChildren<MeshRenderer>().sharedMaterial.mainTexture;
            string textureName = null;
            string transparency = "false";
            if (objectTexture != null && childToAdd.tag!="Model")
            {
                textureName = objectTexture.name;
                byte[] bytes = objectTexture.EncodeToPNG();
                if (!Directory.Exists(folderPath + "textures/")) Directory.CreateDirectory(folderPath + "textures/");
                File.WriteAllBytes(folderPath + "textures/" + textureName + ".png", bytes);
                transparency = "true";
            }
            

            switch (childToAdd.tag)
            {
                case "Plane":
                    string planeID = childToAdd.name.ToLower() + "_" + i;
                    string planeAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + planeID + ".txt");
                    KeyFrameList planeKeyList = JsonUtility.FromJson<KeyFrameList>(planeAnimationFile);

                    Plane newPlane = new Plane();
                    newPlane.initialize();
                    newPlane.setPropertyValues(childToAdd, textureName, planeID);
                    sb.AppendLine(newPlane.getHtmlString(planeKeyList));
                    break;

                case "Video":
                    string videoID = childToAdd.name.ToLower() + "_" + i;
                    string videoAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + videoID + ".txt");
                    KeyFrameList videoKeyList = JsonUtility.FromJson<KeyFrameList>(videoAnimationFile);

                    Video newVideo = new Video();
                    newVideo.initialize();
                    newVideo.setPropertyValues(childToAdd, textureName, videoID);
                    newVideo._src = "#" + childToAdd.name + "_Asset_" + i;
                    sb.AppendLine(newVideo.getHtmlString(videoKeyList));
                    break;

                case "Cube":
                    string cubeID = childToAdd.name.ToLower() + "_" + i;
                    string cubeAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + cubeID + ".txt");
                    KeyFrameList cubeKeyList = JsonUtility.FromJson<KeyFrameList>(cubeAnimationFile);

                    Cube newCube = new Cube();
                    newCube.initialize();
                    newCube.setPropertyValues(childToAdd, textureName, cubeID);
                    sb.AppendLine(newCube.getHtmlString(cubeKeyList));
                    break;

                case "Model":
                    string modelID = childToAdd.name.ToLower() + "_" + i;

                    string modelAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + modelID + ".txt");
                    KeyFrameList modelKeyList = JsonUtility.FromJson<KeyFrameList>(modelAnimationFile);

                    Model newModel = new Model();
                    newModel.initialize();
                    newModel.setPropertyValues(childToAdd, textureName, modelID);
                    newModel.setObjectModelString(i);
                    string modelLineToAppend = newModel.getHtmlString(modelKeyList);
                    Debug.Log(modelLineToAppend);
                    sb.AppendLine(modelLineToAppend);

                    break;

                case "Sphere":
                    string shpereID = childToAdd.name.ToLower() + "_" + i;
                    string sphereAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + shpereID + ".txt");
                    KeyFrameList sphereKeyList = JsonUtility.FromJson<KeyFrameList>(sphereAnimationFile);

                    Sphere newSphere = new Sphere();
                    newSphere.initialize();
                    newSphere.setPropertyValues(childToAdd, textureName, shpereID);
                    sb.AppendLine(newSphere.getHtmlString(sphereKeyList));
                    break;

                case "Cylinder":
                    string cylinderID = childToAdd.name.ToLower() + "_" + i;
                    string cylinderAnimationFile = File.ReadAllText(Application.dataPath + "/Animations/JsonExports/" + SceneManager.GetActiveScene().name + "/" + cylinderID + ".txt");
                    KeyFrameList cylinderKeyList = JsonUtility.FromJson<KeyFrameList>(cylinderAnimationFile);

                    Cylinder newCylinder = new Cylinder();
                    newCylinder.initialize();
                    newCylinder.setPropertyValues(childToAdd, textureName, cylinderID);
                    sb.AppendLine(newCylinder.getHtmlString(cylinderKeyList));
                    break;

                default:

                    break;
            }
        }

        sb.AppendLine("<!-- END: Unity Compiled Objects -->");
        #endregion Unity Compiled Objects

        sb.AppendLine("");
        sb.AppendLine("<!-- BEGIN: Bottom HTML -->");
        sb.Append(HtmlStrings.getBottomMarkerHtml(isNft));
        sb.AppendLine("<!-- END: Bottom HTML -->");

        File.WriteAllText(folderPath + fileName, sb.ToString());
        Debug.Log("index file successfully created");
        AssetDatabase.Refresh();
    }
}