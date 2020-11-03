using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    /// <summary>
    /// Representaion of base class for frame source, used by RBOT
    /// </summary>
    public interface IFrameSource
    {

        Resolution Res { get; }
        Texture Texture { get; }

        /// <summary>
        /// Get a block of pixel colors in Color32 format.
        /// </summary>
        /// <returns></returns>
        Color32[] GetFrameData32();
    }
}