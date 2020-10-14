using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    public enum ResultCode
    {
        Successfull = 0,
        QTAppFailed = -6, 
        NoObjectsToDetect = -2, 
        EmptyFrame = -3,
        FileNotFound = -5,
        PoseEstimatorNoFound = -1
    }

    public class Controller
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

            // Reference to List of tracking objects
            private List<Object3D> _objects;

            public PoseEstimator(List<Object3D> objects, int camera_width, int camera_height, float inZNear, float inZFar, float[] inK, float[] inDistCoeffs, float[] inDistances)
            {
                _cameraWidth = camera_width;
                _cameraHeight = camera_height;
                _zFar = inZFar;
                _zNear = inZNear;
                _K = inK;
                _distCoeffs = inDistCoeffs;
                _distances = inDistances;
                _objects = objects;
            }

            public void EstimatePoses(out ResultCode result, bool undistortFrame, bool checkForLoss)
            {
                int resCode;
                float[] poseData = new float[_objects.Count * 16];
                unsafe
                {
                    fixed (float* outPoseDataPtr = poseData)
                    {
                        resCode = Interop.EstimatePoses(outPoseDataPtr, undistortFrame, checkForLoss);
                    }
                }

                result = (ResultCode)resCode;

                // Update all objects' poses
                if (resCode == 1)
                {
                    for (int i = 0; i < _objects.Count; ++i)
                    {
                        for (int j = 0; j < 16; ++j)
                        {
                            _objects[i].TRS[j / 4, j % 4] = poseData[16 * i + j];
                        }
                    }
                }
            }
            public void ToggleTracking(out ResultCode result, int index, bool undistortFrame = true)
            {
                int resCode = Interop.ToggleTracking(index, undistortFrame);

                if (resCode == 1)
                {
                    _objects[index].isTracking = !_objects[index].isTracking;
                }

                result = (ResultCode)resCode;
            }
        }

        private static class Interop
        {
            [DllImport("RBOT")]
            internal static extern unsafe int CreatePoseEstimator(int camera_width, int camera_height, float inZNear, float inZFar, float* inK, float* inDistCoeffs, float* inDistances);
            [DllImport("RBOT")]
            internal static extern unsafe int ToggleTracking(int objectIndex, bool undistortFrame);
            [DllImport("RBOT")]
            internal static extern unsafe int EstimatePoses(float* outPoseData, bool undistortFrame, bool checkForLoss);
            [DllImport("RBOT")]
            internal static extern unsafe int AddObj3d(StringBuilder path, float tx, float ty, float tz, float alpha, float beta, float gamma, float scale, float qualityThreshold, float* templateDistances);
            [DllImport("RBOT")]
            internal static extern int RemoveObj3d(int index);
            [DllImport("RBOT")]
            internal static extern unsafe int TextureToCVMat(IntPtr frame, int height, int width);
            [DllImport("RBOT")]
            internal static extern int Reset();
        }

        private static Controller _instance = null;
        public static Controller Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Controller();
                return _instance;
            }
        }

        private PoseEstimator _poseEstimator;
        public PoseEstimator poseEstimator { get { return _poseEstimator; } }

        private List<Object3D> objects;

        private Controller()
        {
            objects = new List<Object3D>();
        }

        private void SetNewPoses(float[] poseData)
        {
            for (int i = 0; i < this.objects.Count; ++i)
            {
                for (int j = 0; j < 16; ++j)
                {
                    objects[i].TRS[j / 4, j % 4] = poseData[i * 16 + j];
                }
            }
        }

        // Initialize pose estimator 
        public void CreateNewPoseEstimator(out ResultCode result, int camera_width, int camera_height, float inZNear, float inZFar, float[] K, float[] distCoeffs, float[] distances)
        {
            int resCode;

            unsafe
            {
                fixed (float* ptrK = K, ptrDistCoeffs = distCoeffs, ptrDistances = distances)
                {
                    resCode = Interop.CreatePoseEstimator(camera_width, camera_height, inZNear, inZFar, ptrK, ptrDistCoeffs, ptrDistances);
                }
            }

            if (resCode == 1)
            {
                this._poseEstimator = new PoseEstimator(objects, camera_width, camera_height, inZNear, inZFar, K, distCoeffs, distances);
            }
            result = (ResultCode)resCode;    
        }
        // Add a new 3D-model in Obj-format to track
        public void AddObject(out ResultCode result, string fullFileName, float tx, float ty, float tz, float alpha, float beta, float gamma, float scale, float qualityThreshold, float[] templateDistances)
        {
            int resCode;
            Object3D tmpObj = new Object3D(fullFileName, tx, ty, tz, alpha, beta, gamma, scale);
            objects.Add(tmpObj);

            StringBuilder tmpStr = new StringBuilder(tmpObj.FullFileName);
            unsafe
            {
                fixed (float* templateDistancesPtr = templateDistances)
                {
                    resCode = Interop.AddObj3d(tmpStr, tx, ty, tz, alpha, beta, gamma, scale, qualityThreshold, templateDistancesPtr);
                }
            }

            result = (ResultCode)resCode;
        }
        public void RemoveObject(out ResultCode result, int index)
        {
            int resCode = Interop.RemoveObj3d(index);

            result = (ResultCode)resCode;
        }
        public void TextureToCVMat(out ResultCode result, Color32[] textureData, int height, int width)
        {
            int resCode;
            unsafe
            {
                fixed(Color32* textDataPtr = textureData)
                {
                   resCode = Interop.TextureToCVMat((IntPtr)textDataPtr, height, width);
                }
            }

            result = (ResultCode)resCode;
        }
    }   
}
