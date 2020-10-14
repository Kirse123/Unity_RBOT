using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    private Button InitBtn;
    [SerializeField]
    private Button PoseEstimationBtn;
    [SerializeField]
    private Button CloseBtn;
    [SerializeField]
    private Text fpsCounter;

    // Start is called before the first frame update
    void Awake()
    {
        if (UIController.instance == null)
            UIController.instance = this;
    }

    private void Start()
    {
        InitBtn.onClick.AddListener(OnInitClicked);
        PoseEstimationBtn.onClick.AddListener(OnEstimateClicked);
        CloseBtn.onClick.AddListener(OnCloseBtnClicked);
        fpsCounter.text = "";
    }

    private void OnInitClicked()
    {
        Debug.Log("Click!!!");
        RBOT_Controller.instance.Init();
    }

    private void OnEstimateClicked()
    {
        Debug.Log("Click!!!   ");
        RBOT_Controller.instance.EstimatePose();
    }

    private void OnCloseBtnClicked()
    {
        RBOT_Controller.instance.Close();
    }
    private void OnPoseEstimated(RBOT_Controller.RBOT_ResultCode result)
    {
        
    }
}
