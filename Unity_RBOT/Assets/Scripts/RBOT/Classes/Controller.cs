using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace RBOT_UnityPlugin
{
    public interface IRBOTCommand
    {
        void Execute();
    }

    public enum ResultCode
    {
        Successful = 0,
        QTAppFailed = -6, 
        NoObjectsToDetect = -2, 
        EmptyFrame = -3,
        FileNotFound = -5,
        PoseEstimatorNoFound = -1
    }

    public class Controller
    {
        public static event RBOT_PoseEstimationEventHandler PoseEstimationEvent;
        public static event RBOT_ObjectAddedEventHandler ObjectAddedEvent;
        public static event RBOT_ObjectRomevedEventHandler ObjectRomevedEvent;
        public static event RBOT_PoseEstimatorCreatedEventHandler PoseEstimatorCreatedEvent;

        private static Controller _instance = null;
        private static readonly object padlock = new object();
        public static Controller Instance
        {
            get
            {
                // Locking for thread-safety reasons
                lock (padlock)
                {
                    if (_instance == null)
                        _instance = new Controller();
                    return _instance;
                }
            }
        }

        private PoseEstimator _poseEstimator;

        private List<Object3D> objects;
        public List<Object3D> Objects { get { return objects; } }

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
        private void CreateNewPoseEstimator(out ResultCode result, int camera_width, int camera_height, float inZNear, float inZFar, float[] K, float[] distCoeffs)
        {
            int resCode;

            unsafe
            {
                fixed (float* ptrK = K, ptrDistCoeffs = distCoeffs)
                {
                    resCode = Interop.CreatePoseEstimator(camera_width, camera_height, inZNear, inZFar, ptrK, ptrDistCoeffs);
                }
            }

            if (resCode == (int)ResultCode.Successful)
            {
                this._poseEstimator = new PoseEstimator(objects, camera_width, camera_height, inZNear, inZFar, K, distCoeffs);
            }
            result = (ResultCode)resCode;

            PoseEstimatorCreatedEvent.Invoke(new RBOT_PoseEstimatorCreatedEventArgs("Estimator created", result));
        }
        // Add a new 3D-model in Obj-format to track
        private void AddObject(out ResultCode result, string fullFileName, float tx, float ty, float tz, float alpha, float beta, float gamma, float scale, float qualityThreshold, float[] templateDistances)
        {
            int resCode;
            Object3D tmpObj = null;

            StringBuilder tmpStr = new StringBuilder(fullFileName);
            unsafe
            {
                fixed (float* templateDistancesPtr = templateDistances)
                {
                    resCode = Interop.AddObj3d(tmpStr, tx, ty, tz, alpha, beta, gamma, scale, qualityThreshold, templateDistancesPtr);
                }
            }            

            result = (ResultCode)resCode;
            if (result == ResultCode.Successful)
            {
                tmpObj = new Object3D(fullFileName, tx, ty, tz, alpha, beta, gamma, scale);
                objects.Add(tmpObj);
            }

            ObjectAddedEvent.Invoke(new RBOT_ObjectAddedEventArgs("Object added", (ResultCode)resCode, tmpObj));
        }
        private void RemoveObject(out ResultCode result, int index)
        {
            int resCode = Interop.RemoveObj3d(index);

            result = (ResultCode)resCode;

            if(result == ResultCode.Successful)
            {
                ObjectRomevedEvent.Invoke(new RBOT_ObjectRemovedEventArgs("Object removed", (ResultCode)resCode, index));
                objects.RemoveAt(index);
            }            
        }
        private void TextureToCVMat(out ResultCode result, Color32[] textureData, int height, int width)
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
        private void ResetPoses()
        {
            Interop.Reset();
        }
        private void Close()
        {
            Interop.Close();
        }

        private class PoseEstimator
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

            // Reference to List of tracking objects
            private List<Object3D> _objects;

            public PoseEstimator(List<Object3D> objects, int camera_width, int camera_height, float inZNear, float inZFar, float[] inK, float[] inDistCoeffs)
            {
                _cameraWidth = camera_width;
                _cameraHeight = camera_height;
                _zFar = inZFar;
                _zNear = inZNear;
                _K = inK;
                _distCoeffs = inDistCoeffs;
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
                if (resCode == (int)ResultCode.Successful)
                {
                    for (int i = 0; i < _objects.Count; ++i)
                    {
                        for (int j = 0; j < 16; ++j)
                        {
                            _objects[i].TRS[j / 4, j % 4] = poseData[16 * i + j];
                        }
                        //Debug.Log(_objects[i].TRS);
                    }
                }

                PoseEstimationEvent.Invoke(new RBOT_PoseEstimationEventArgs("Pose estimated", (ResultCode)resCode, poseData));
            }
            public void ToggleTracking(out ResultCode result, int index, bool undistortFrame = true)
            {
                int resCode = Interop.ToggleTracking(index, undistortFrame);

                if (resCode == (int)ResultCode.Successful)
                {
                    _objects[index].isTracking = !_objects[index].isTracking;
                }

                result = (ResultCode)resCode;
            }
        }
        private static class Interop
        {
            [DllImport("RBOT")]
            internal static extern unsafe int CreatePoseEstimator(int camera_width, int camera_height, float inZNear, float inZFar, float* inK, float* inDistCoeffs);
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
            [DllImport("RBOT")]
            internal static extern void Close();
        }

        // Command classes
        public class ToggleTrcakingCmd : IRBOTCommand
        {
            int _index;
            bool _undistortFrame;

            public ToggleTrcakingCmd(int index, bool undistortFrame = true)
            {
                _index = index;
                _undistortFrame = undistortFrame;
            }

            public void Execute()
            {
                ResultCode result;

                Controller.Instance._poseEstimator.ToggleTracking(out result, _index, _undistortFrame);
            }
        }
        public class EstimatePosesCmd : IRBOTCommand
        {
            bool _undistortFrame, _checkForLoss;
            int _width, _height;
            Color32[] _frameData;

            public EstimatePosesCmd(Color32[] frameData, int width, int height, bool undistortFrame, bool checkForLoss)
            {
                _checkForLoss = checkForLoss;
                _undistortFrame = undistortFrame;
                _width = width;
                _height = height;
                _frameData = frameData;
            }
            public void Execute()
            {
                ResultCode result;

                Controller.Instance.TextureToCVMat(out result, _frameData, _height, _width);

                if (result == ResultCode.Successful)
                    Controller.Instance._poseEstimator.EstimatePoses(out result, _undistortFrame, _checkForLoss);
            }
        }
        public class AddObjectCmd : IRBOTCommand
        {
            string _fullFileName;
            float _tx, _ty, _tz, _alpha, _beta, _gamma, _scale, _qualityThreshold;
            float[] _templateDistances;

            public AddObjectCmd(string fullFileName, float tx, float ty, float tz, float alpha, float beta, float gamma, float scale, float qualityThreshold, float[] templateDistances)
            {
                _fullFileName = fullFileName;
                _tx = tx;
                _ty = ty;
                _tz = tz;
                _alpha = alpha;
                _beta = beta;
                _gamma = gamma;
                _scale = scale;
                _qualityThreshold = qualityThreshold;
                _templateDistances = templateDistances;
            }

            public void Execute()
            {
                ResultCode result;

                Controller.Instance.AddObject(out result, _fullFileName, _tx, _ty, _tz, _alpha, _beta, _gamma, _scale, _qualityThreshold, _templateDistances);
            }
        }
        public class RemoveObjectCmd : IRBOTCommand
        {
            int _index;

            public RemoveObjectCmd(int index)
            {
                _index = index;
            }

            public void Execute()
            {
                ResultCode result;

                Controller.Instance.RemoveObject(out result, _index);
            }
        }
        public class CreateNewPoseEstimatorCmd : IRBOTCommand
        {
            int _camera_width, _camera_height;
            float _inZNear, _inZFar;
            float[] _K, _distCoeffs;

            public CreateNewPoseEstimatorCmd(int camera_width, int camera_height, float inZNear, float inZFar, float[] K, float[] distCoeffs)
            {
                _camera_width = camera_width;
                _camera_height = camera_height;
                _inZNear = inZNear;
                _inZFar = inZFar;
                _K = K;
                _distCoeffs = distCoeffs;
            }
            public void Execute()
            {
                ResultCode result;
                Controller.Instance.CreateNewPoseEstimator(out result, _camera_width, _camera_height, _inZNear, _inZFar, _K, _distCoeffs);
            }
        }
        public class ResetCmd : IRBOTCommand
        {
            public void Execute()
            {
                Controller.Instance.ResetPoses();
            }
        }
        public class CloseCmd : IRBOTCommand
        {
            public void Execute()
            {
                Controller.Instance.Close();
            }
        }
    }   
}