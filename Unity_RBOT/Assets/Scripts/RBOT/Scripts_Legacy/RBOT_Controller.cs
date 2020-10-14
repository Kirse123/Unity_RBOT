using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using RBOT_UnityPlugin;

public class RBOT_Controller : MonoBehaviour
{

    public enum RBOT_ResultCode
    {
        Successfully = 0,
        Failed = -1,
        FileNotFound = -2,
        EmptyFrame = -3

    }
    public bool DebugMode;
    private Texture2D _testImage;
    private bool inProgress = false;
    private bool objLoaded = false;

    /// <summary>
    /// Contains a singleton reference
    /// </summary>
    public static RBOT_Controller instance { get; set; }

    private Matrix4x4 _currentTRS_Matrix;
    public Matrix4x4 CurrentTRS_Matrix { get { return _currentTRS_Matrix; } }

    private bool _isInitialized = false;
    /// <summary>
    /// Current status of RBOT
    /// </summary>
    public bool IsInitialized { get { return _isInitialized; } }

    private Resolution _cameraResolution;
    /// <summary>
    /// Stores current physycal camera resolution
    /// </summary>
    public Resolution CameraResolution { get { return _cameraResolution; } }

    //private string _pathToModels = System.IO.Path.Combine(Application.persistentDataPath, "objModels");
    private string _pathToModels;
    public string PathToModels { get { return _pathToModels; } }

    /// <summary>
    /// Contains frame source for paose estimation. Might be a Web-cam or static image
    /// </summary>
    private FrameSource _frameSource;
    public FrameSource FrameSource { get { return _frameSource;  } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    public delegate void OnRBOT_Event(RBOT_ResultCode result);
    public event OnRBOT_Event RBOT_PoseEstimated;
    public event OnRBOT_Event RBOT_Initialized;
    public event OnRBOT_Event RBOT_ObjAdded;

    /// <summary>
    /// Commits RBOT initialization by starting a coroutine
    /// </summary>
    public void Init()
    {
        Debug.Log("Initializing RBOT...");
        StringBuilder tmpStr = new StringBuilder(PathToModels);
        int result = RBOT_Interop.Init(this.CameraResolution.width, this.CameraResolution.height, tmpStr, tmpStr.Capacity);
        RBOT_Initialized.Invoke((RBOT_ResultCode)result);
        this._isInitialized = result == 0;
    }
    /// <summary>
    /// Destoys the RBOT native instance 
    /// </summary>
    public void Close()
    {
        RBOT_Interop.Close();
    }
    /// <summary>
    /// Estimates object pose on current frame
    /// </summary>
    public void EstimatePose()
    {
        // check if initialize and not currently in progress
        if (this.IsInitialized && !this.inProgress && this.objLoaded)
        {
            this.inProgress = true;
            StartCoroutine(EstimatePoseCoroutine());
        }
    }
    /// <summary>
    /// Add load the obj 3D-model inti RBOT-dll
    /// </summary>
    /// <param name="fileNameWithExt">3D-model file's name with an extension saved on disk</param>
    public void AddObj(string fileNameWithExt)
    {
        StringBuilder tmpStr = new StringBuilder(fileNameWithExt);
        System.Threading.Thread thread = new System.Threading.Thread(
            delegate ()
            {
                int result = -1;
                result = RBOT_Interop.AddObj(tmpStr, tmpStr.Capacity);
                RBOT_ObjAdded.Invoke((RBOT_ResultCode)result);
            });
        thread.Start();
    }

    private void Awake()
    {
        if (RBOT_Controller.instance == null)
            RBOT_Controller.instance = this;
    }
    private void Start()
    {
        //_pathToModels = string.Concat(System.IO.Path.GetFullPath(Application.dataPath), "\\Resources\\RBOT_Models");
        _pathToModels = "E:\\data";             //works. Dev folder
        Debug.Log("Model's path: " + _pathToModels);

        _currentTRS_Matrix = new Matrix4x4();
        RBOT_Initialized += ShowInitRes;      
        RBOT_PoseEstimated += ShowEstimationRes;
        RBOT_ObjAdded += OnObjAdded;

        if (DebugMode)
        {
            _frameSource = new StaticImage(string.Concat(Application.dataPath, "/Resources/Images/frame.png"), 640, 512);

            _cameraResolution = new Resolution();
            _cameraResolution.width = _testImage.width;
            _cameraResolution.height = _testImage.height;
        }
    }
    private void Update()
    {
        if (objLoaded && !this.inProgress)
            EstimatePose();
    }
    private void OnApplicationQuit()
    {
        if (this._isInitialized)
            this.Close();
    }

    private IEnumerator EstimatePoseCoroutine()
    {
        TextureToCVMat();
        PoseEstimatorAsync poseEstimator = new PoseEstimatorAsync();
        Debug.Log("Started...");
        poseEstimator.Start();
        while (!poseEstimator.Update())
        {
            yield return null;
        }
        _currentTRS_Matrix.SetValuesFromArray(poseEstimator.OutData);
        Debug.Log(_currentTRS_Matrix);
        RBOT_PoseEstimated.Invoke((RBOT_ResultCode)poseEstimator.Result);
        poseEstimator = null;
        //this.inProgress = false;
    }
    private IEnumerator InitCoroutine(int camera_width, int camera_height, string path) 
    {
        RBOT_Initializer rbot_Initializer = new RBOT_Initializer(camera_width, camera_height, path);
        Debug.Log("Initializing RBOT...");
        rbot_Initializer.Start();
        while (!rbot_Initializer.Update())
        {
            yield return null;
        }

        _isInitialized = rbot_Initializer.Result == 0;


        RBOT_Initialized.Invoke((RBOT_ResultCode)rbot_Initializer.Result);

        rbot_Initializer = null;
        this.inProgress = false;        
    }
    private unsafe void TextureToCVMat()
    {
        fixed (Color32* ptr = FrameSource.GetFrameData32())
        {
            RBOT_Interop.TextureToCVMat((IntPtr)ptr, FrameSource.Resolution.height, FrameSource.Resolution.width);
        }
    }


    // Callbacks
    private void ShowInitRes(RBOT_ResultCode result)
    {
        Debug.LogFormat("RBOT initializing completed with result: {0}", result);
        if (result == RBOT_ResultCode.Successfully)
        {
            Debug.Log("Adding model");
            //AddObj("squirrel_demo_low.obj");
            objLoaded = true;
        }
    }
    private void ShowEstimationRes(RBOT_ResultCode result)
    {
        Debug.LogFormat("RBOT Estimation result {0}", result);
    }
    private void OnObjAdded(RBOT_ResultCode result)
    {
        Debug.LogFormat("Loading model completed with result: {0}", result);
        if (result == RBOT_ResultCode.Successfully)
            objLoaded = true;
    }
}

public static class RBOT_Interop
{
    [DllImport("RBOT")]
    internal static extern unsafe int GetPose(float* outData);
    [DllImport("RBOT")]
    internal static extern unsafe int Init(int camera_width, int camera_height, StringBuilder path, int pathLength);
    [DllImport("RBOT")]
    internal static extern unsafe int AddObj(StringBuilder path, int pathLength);
    [DllImport("RBOT")]
    internal static extern unsafe int TextureToCVMat(IntPtr frame, int height, int width);
    [DllImport("RBOT")]
    internal static extern void Close();
}
