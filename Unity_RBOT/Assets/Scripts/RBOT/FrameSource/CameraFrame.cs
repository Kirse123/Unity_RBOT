using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of frame source as current web-camera frame
    public class CameraFrame : IFrameSource
    {
        private WebCamTexture _webCam;
        public Texture Texture { get { return _webCam; } }

        private Resolution _res;
        public Resolution Res
        {
            get
            {
                return _res;
            }
        }

        /// <summary>
        /// Construct new frame source, connected to web-camera
        /// </summary>
        /// <param name="webCam"></param>
        public CameraFrame(WebCamTexture webCam)
        {
            this._webCam = webCam;
            _res = new Resolution();
            _res.width = webCam.width;
            _res.height = webCam.height;
        }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        public Color32[] GetFrameData32()
        {
            return _webCam.GetPixels32();
        }
    }
}