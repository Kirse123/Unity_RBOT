using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of base class for frame source, used by RBOT
    /// </summary>
    public abstract class FrameSource
    {
        protected Resolution _resolution;
        public Resolution Resolution { get { return _resolution; } }

        protected FrameSource(int width, int height)
        {
            _resolution = new Resolution();
            _resolution.width = width;
            _resolution.height = height;
        }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        public abstract Color32[] GetFrameData32();
    }
}