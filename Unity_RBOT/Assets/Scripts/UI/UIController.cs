using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [SerializeField]
    private Button PauseBtn;
    [SerializeField]
    private Button ResumeBtn; 
    [SerializeField]
    private Button ResetBtn;
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
        PauseBtn.onClick.AddListener(OnPauseBtnClicked);
        ResumeBtn.onClick.AddListener(OnResumeBtnClicked);
        ResetBtn.onClick.AddListener(OnResetBtnClicked);
        fpsCounter.text = "";
    }

    private void Update()
    {
        fpsCounter.text = Mathf.Round(1f / Time.unscaledDeltaTime).ToString() + " FPS";
    }

    private void OnResetBtnClicked()
    {
        RBOT_SceneController.Instance.ResetTracking();
        PauseBtn.gameObject.SetActive(false);
        ResumeBtn.gameObject.SetActive(true);
    }
    private void OnPauseBtnClicked()
    {
        PauseBtn.gameObject.SetActive(false);
        RBOT_SceneController.Instance.PauseTracking();
        ResumeBtn.gameObject.SetActive(true);
    }
    private void OnResumeBtnClicked()
    {
        ResumeBtn.gameObject.SetActive(false);
        RBOT_SceneController.Instance.ResumeTracking();
        PauseBtn.gameObject.SetActive(true);
    }
}
