using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of frame source as current web-camera frame
    public class CameraFrame : FrameSource
    {
        private WebCamTexture _webCam;
        /// <summary>
        /// Construct new frame source, connected to web-camera
        /// </summary>
        /// <param name="webCam"></param>
        public CameraFrame(WebCamTexture webCam) : base(webCam.width, webCam.height)
        {
            this._webCam = webCam;
        }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        public override Color32[] GetFrameData32()
        {
            return _webCam.GetPixels32();
        }
    }
}