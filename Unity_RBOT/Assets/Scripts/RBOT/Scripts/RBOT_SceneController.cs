using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RBOT_UnityPlugin;

public class RBOT_SceneController : MonoBehaviour
{
    [SerializeField]
    private string[] models;

    private bool trackingIsON = false;

    private ThreadedJob threadedJob;

    private static RBOT_SceneController _instance;
    public static RBOT_SceneController Instance
    {
        get
        {
            return _instance;
        }
    }

    private ARCamera ARCamera;

    void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe for events
        Controller.ObjectAddedEvent += OnObjectAdded;
        Controller.PoseEstimatorCreatedEvent += OnPoseEstimatorCreated;
        Controller.PoseEstimationEvent += OnPosesEstimeted;
        //Controller.Instance.CreateNewPoseEstimator()
        
        // test
        this.AddObject(System.IO.Path.Combine("Assets", "Resources", "Models", "squirrel_demo_low.obj"));
        ARCamera = Camera.main.GetComponent<ARCamera>();
        this.CreateNewPoseEstimator();
        this.EstimatePoses(false, true);
        this.ToggleTracking(0);
        this.EstimatePoses(false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(trackingIsON)
        {
            EstimatePoses(false, true);
        }
    }

    public void CreateNewPoseEstimator()
    {
        float[] K = { 650.048f, 0f, 324.328f, 0f, 647.183f, 257.323f, 0f, 0f, 1f };
        float[] distCoeffs = { 0.0f, 0.0f, 0.0f, 0.0f };
        int width = 640;
        int height = 512;
        Camera.main.GetComponent<IntrinsicSetup>().SetCameraParam(K, width, height);
        new Controller.CreateNewPoseEstimatorCmd(width, height, 10f, 10000f, K, distCoeffs).Execute();
        //threadedJob = new ThreadedJob(new Controller.CreateNewPoseEstimatorCmd(width, height, 10f, 10000f, K, distCoeffs));
        //threadedJob.Start();
    }
    public void OnPoseEstimatorCreated(RBOT_PoseEstimatorCreatedEventArgs e)
    {
        Debug.LogFormat("Pose estimator created with result {0}", e.Result.ToString());
    }

    public void AddObject(string modelName)
    {
        // test
        float[] tmpDistances = { 200.0f, 400.0f, 600.0f };

        new Controller.AddObjectCmd(modelName, 15, -35, 515, 55, -20, 205, 1.0f, 0.55f, tmpDistances).Execute();
        //threadedJob = new ThreadedJob(new Controller.AddObjectCmd(modelName, 15, -35, 515, 55, -20, 205, 1.0f, 0.55f, tmpDistances));
        //threadedJob.Start();
    }
    public void OnObjectAdded(RBOT_ObjectAddedEventArgs e)
    {
        Debug.LogFormat("Object {0} added with result {1}", e.object3D.FullFileName, e.Result.ToString());
    }

    public void EstimatePoses(bool undistortFrame, bool checkForLoss)
    { 
        Color32[] tmp = ARCamera.GetFrameSource.GetFrameData32();
        
        new Controller.EstimatePosesCmd(tmp, ARCamera.GetFrameSource.Res.width, ARCamera.GetFrameSource.Res.height, undistortFrame, checkForLoss).Execute();
        //threadedJob = new ThreadedJob(new Controller.EstimatePosesCmd(tmp, ARCamera.GetFrameSource.Res.width, ARCamera.GetFrameSource.Res.height, undistortFrame, checkForLoss));
        //threadedJob.Start();
    }
    private void OnPosesEstimeted(RBOT_PoseEstimationEventArgs e)
    {
        Debug.LogFormat("Estimation complete with result {0}", e.Result.ToString());       
    }

    public void ToggleTracking(int index)
    {
        new Controller.ToggleTrcakingCmd(0, false).Execute();
    }
    public void ResetTracking()
    {
        new Controller.ResetCmd().Execute();
    }

    private void OnApplicationQuit()
    {
        new Controller.CloseCmd().Execute();
    }
    public void PauseTracking()
    {
        this.trackingIsON = false;
    }
    public void ResumeTracking()
    {
        this.trackingIsON = true;
    }
}
