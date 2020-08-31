using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBOT_Controller : MonoBehaviour
{
    private bool inProgress = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTestClicked()
    {
        Debug.Log("Clicked");
        if (!inProgress)
        {
            StartCoroutine(this.EstimatePose());
            inProgress = true;
        }
    }

    private IEnumerator EstimatePose()
    {
        PoseEstimatorAsync poseEstimator = new PoseEstimatorAsync();
        Debug.Log("Started...");
        poseEstimator.Start();
        while (!poseEstimator.Update())
        {
            yield return null;
        }
        Debug.LogFormat("Result: {0}", poseEstimator.Result);
        for(int i = 0; i < 4; ++i)
            for (int j = 0; j < 4; ++j)
                Debug.Log(poseEstimator.OutData[i * 4 + j]);
        poseEstimator = null;
        inProgress = false;
    }
}
