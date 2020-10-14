using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseEstimatorAsync : ThreadedJob
{
    public float[] _outData;
    public float[] OutData { get { return _outData; } }

    private int _result = -1;
    public int Result { get { return _result; } }

    public PoseEstimatorAsync(int outDataSize = 16)
    {
        _outData = new float[outDataSize];
    }

    protected override void ThreadFunction()
    {
        unsafe
        {
            fixed(float* ptr = _outData)
            {
                this._result = RBOT_Interop.GetPose(ptr);
            }
        }
    }
    protected override void OnFinished()
    {
        base.OnFinished();
    }
}
