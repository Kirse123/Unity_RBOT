using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBOT_UnityPlugin
{

    public class PoseEstimator
    {
        private int _cameraWidth;
        public int CameraWidth { get { return _cameraWidth; } }

        private int _cameraHeight;
        public int CameraHeight { get { return _cameraHeight; } }

        private float _zNear;
        public float zNear { get { return _zNear; } }

        private float _zFar;
        public float zFar { get { return _zFar; } }

        private float[] _K;
        public float[] K { get { return _K; } }

        private float[] _distCoeffs;
        public float[] DistCoeffs { get { return _distCoeffs; } }

        private float[] _distances;
        public float[] Distances { get { return _distances; } }

        public PoseEstimator(int camera_width, int camera_height, float inZNear, float inZFar, float[] inK, float[] inDistCoeffs, float[] inDistances)
        {
            _cameraWidth = camera_width;
            _cameraHeight = camera_height;
            _zFar = inZFar;
            _zNear = inZNear;
            _K = inK;
            _distCoeffs = inDistCoeffs;
            _distances = inDistances;
        }

        public void EstimatePoses()
        {

        }

        public void ToggleTracking(int objectID, bool undistortFrame = true)
        {

        }
    }
}
