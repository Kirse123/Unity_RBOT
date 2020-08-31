using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PoseEstimatorAsync : ThreadedJob
{
    private float[] _outData;
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
            fixed(float* ptr = this._outData)
            {
                this._result = RBOT_Interop.GetPose(ptr);
            }
        }
    }
}

internal static class RBOT_Interop
{
    [DllImport("RBOT")]
    internal static extern unsafe int GetPose(float* outData);
}
