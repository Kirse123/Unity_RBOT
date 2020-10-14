using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of frame source as static image on the disk
    /// </summary>
    public class StaticImage : FrameSource
    {
        private Texture2D _image;

        /// <summary>
        /// Construct frame source as static image on disk
        /// </summary>
        /// <param name="pathToImage">location from disk</param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        public StaticImage(string pathToImage, int imgWidth, int imgHeight) : base(imgWidth, imgHeight)
        {
            byte[] imgDate = System.IO.File.ReadAllBytes(pathToImage);
            this._image = new Texture2D(imgWidth, imgHeight);
            this._image.LoadImage(imgDate);
        }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        public override Color32[] GetFrameData32()
        {
            return this._image.GetPixels32();
        }
    }
}