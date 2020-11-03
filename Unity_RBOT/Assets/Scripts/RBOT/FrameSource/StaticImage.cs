using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of frame source as static image on the disk
    /// </summary>
    public class StaticImage : IFrameSource
    {
        private Texture2D _image;
        public Texture Texture { get { return _image; } }

        private Resolution _res;
        public Resolution Res
        {
            get
            {
                return _res;
            }
        }

        /// <summary>
        /// Construct frame source as static image on disk
        /// </summary>
        /// <param name="pathToImage">location from disk</param>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        public StaticImage(string pathToImage, int imgWidth, int imgHeight)
        {
            byte[] imgDate = System.IO.File.ReadAllBytes(pathToImage);
            this._image = new Texture2D(imgWidth, imgHeight);
            this._image.LoadImage(imgDate);
            _res = new Resolution();
            _res.height = imgHeight;
            _res.width = imgWidth;
        }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        public  Color32[] GetFrameData32()
        {
            return this._image.GetPixels32();
        }
    }
}