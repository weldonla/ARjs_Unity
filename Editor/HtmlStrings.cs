using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HtmlStrings
{
    // Imports
    // newer versions that i can't use yet 
    // <script src=\"https://aframe.io/releases/1.0.4/aframe.min.js\"></script>
    // <script src=\"https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar.js\"></script>
    // older versions i'm switching back to to fix some bugs (only aframe switch was needed)
    // <script src=\"https://aframe.io/releases/0.8.2/aframe.min.js\"></script>
    // <script src=\"../build/aframe-ar.js\"></script>
    public static string aframeMarkerImport = "<script src=\"https://aframe.io/releases/0.8.2/aframe.min.js\"></script>";
    public static string arjsMarkerImport = "<script src=\"https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar.js\"></script>";
    public static string aframeNftImport = "<script src=\"https://cdn.jsdelivr.net/gh/aframevr/aframe@1c2407b26c61958baa93967b5412487cd94b290b/dist/aframe-master.min.js\"></script>";
    public static string arjsNftImport = "<script src=\"https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar-nft.js\"></script>";
    public static string plySupportImport = "<script src=\"https://rawgit.com/donmccurdy/aframe-extras/v6.0.0/dist/aframe-extras.loaders.min.js\"></script>";

    // General HTML strings
    public static string getTopHtml(bool isNft) {
        string aframeImport = isNft ? aframeNftImport : aframeMarkerImport;
        string arjsImport = isNft ? arjsNftImport : arjsMarkerImport;

        return $"<!DOCTYPE html>\n<!-- include aframe -->\n{aframeImport}\n<!-- include ar.js -->\n{arjsImport}\n\n<!-- to load .ply model -->\n{plySupportImport}\n\n";
    }
    public static string bodyHtml = @"<body style='margin : 0px; overflow: hidden; font-family: Monospace;'>";
    public static string middleHTML = @"
        <!-- <a-scene embedded arjs='debugUIEnabled: false; sourceType: video; sourceUrl:../../data/videos/headtracking.mp4;'> -->
            <a-scene embedded arjs='debugUIEnabled: false; sourceType: webcam' vr-mode-ui='enabled: false'>
            <a-entity id=""mouseCursor"" cursor=""rayOrigin: mouse"" raycaster=""objects: .intersectable; useWorldCoordinates: true;""></a-entity>";
    public static string buttonHTML = @"
        <div style='position: absolute; bottom: 10px; right: 30px; width:100%; text-align: center; z-index: 1;'>      
            <button id=""mutebutton"" style='position: absolute; bottom: 10px'>
                Unmute
            </button>
        </div>";
    public static string fullscreenButtonHTML = @"
        <div style='position: absolute; bottom: 5px; left: 30px; width:100%; text-align: right; z-index: 1;'>
            <input type=""image"" id=""fullscreen"" src=""../fullscreen.png"" style='position: absolute; bottom: 0px; right: 35px;'>
            </input>
        </div>";
    public static string fullscreenButtonActionHTML = @"
        fullbutton.addEventListener(""click"", function (evt) {
            if (fullscreen == 0) {
                if (elem.requestFullscreen) {
                    elem.requestFullscreen();
            } else if (elem.mozRequestFullScreen) {
                    /* Firefox */
                elem.mozRequestFullScreen();
            } else if (elem.webkitRequestFullscreen) {
                    /* Chrome, Safari and Opera */
                elem.webkitRequestFullscreen();
            } else if (elem.msRequestFullscreen) {
                    /* IE/Edge */
                elem.msRequestFullscreen();
            }
            fullbutton.setAttribute(""src"", ""../exit_fullscreen.png"");
            fullscreen = 1;
            } else {
                    if (document.exitFullscreen) {
                        document.exitFullscreen();
                } else if (document.webkitExitFullscreen) {
                        document.webkitExitFullscreen();
                } else if (document.mozCancelFullScreen) {
                        document.mozCancelFullScreen();
                } else if (document.msExitFullscreen) {
                        document.msExitFullscreen();
                }
                fullbutton.setAttribute(""src"", ""../fullscreen.png"");
                fullscreen = 0;
            }
        });";

    public static string bezierCurveFunctions = @"
        function bezierEvaluate(p0, p1, p2, p3, t) {
            var u = (1 - t);                
            var uu = u * u;                
            var uuu = u * u * u;               
            var tt = t * t;               
            var ttt = t * t * t;               
            //B(t) = (1-t)^3*P0 3*(1-t)^2*t*P1 3*(1-t)*t^2*P2 t^3*P3 , 0 < t < 1               
            return (uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3);          
        }          
        function bezierPath(p0, p1, p2, p3, t) {               
            return new THREE.Vector3(                   
                bezierEvaluate(p0.x, p1.x, p2.x, p3.x, t),                   
                bezierEvaluate(p0.y, p1.y, p2.y, p3.y, t),                   
                bezierEvaluate(p0.z, p1.z, p2.z, p3.z, t)               
            );           
        }     
    }";

    public static string getBezierPathWithId(string id) {
        return $@"
            function {id}_Update() {{
                var newPosition = bezierPath({id}_PointsArray[{id}_CurrentPoint], {id}_PointsArray[{id}_CurrentPoint + 1], { id}_PointsArray[{ id}_CurrentPoint + 2], { id}_PointsArray[{ id}_CurrentPoint + 3], {id}_Time *{ id}_Speed);
                if ({id}_Time*{id}_Speed>1) {{
                    {id}_CurrentPoint += 3;
                    {id}_SubtractTime = totalTime;
                    if ({id}_CurrentPoint >= {id}_PointsArray.length - 3) {{
                        {id}_CurrentPoint = 0;
                    }}
                }}


                {id}.setAttribute('position', {{
                    x: newPosition.x,
                    y: newPosition.y,
                    z: newPosition.z,
                }});
            }}
        ";
    }
    public static string getTopMarkerHtml(bool isNft) {
        string markerHTML = "";
        if(!isNft) {
            string markerpresetText = $"preset=\"hiro\" emitevents=\"true\" button";
            string patternName = GameObject.FindWithTag("ImageTarget").GetComponent<ImageTarget>().patternName;
            if (patternName != "default") markerpresetText = $"type=\"pattern\" preset=\"custom\" src=\"{patternName}\" url=\"{patternName}\" emitevents=\"true\" button";
            markerHTML = "<a-marker id=\"marker\" " + markerpresetText + ">";
        }
        else {
            ImageTarget it = GameObject.FindWithTag("ImageTarget").GetComponent<ImageTarget>();
            markerHTML = $@"<a-nft
                type=""nft""
                url=""{it.patternName}""
                smooth=""{it.smooth}""
                smoothCount=""{it.smoothCount}""
                smoothTolerance=""{it.smoothTolerance}""
                smoothThreshold=""{it.smoothThreshold}"">";
        }
        
        return markerHTML;
    }
    public static string getBottomMarkerHtml(bool isNft) {
        return isNft ? "</a-nft>\n<a-entity camera></a-entity>\n</a-scene>\n</body>\n</html>" : "</a-marker>\n<a-entity camera></a-entity>\n</a-scene>\n</body>\n</html>";
    }

}
