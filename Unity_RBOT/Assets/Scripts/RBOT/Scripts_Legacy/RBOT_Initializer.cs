using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBOT_Initializer : ThreadedJob
{
    private int _result = -1;
    public int Result { get { return _result; } }

    private Resolution _camreaRes;
    private System.Text.StringBuilder _pathToModels;

    public RBOT_Initializer(int camera_width, int camera_height, string path)
    {
        _camreaRes = new Resolution();
        _camreaRes.width = camera_width;
        _camreaRes.height = camera_height;

        _pathToModels = new System.Text.StringBuilder(path);
    }

    protected override void ThreadFunction()
    {
        this._result = RBOT_Interop.Init(this._camreaRes.width, this._camreaRes.height, _pathToModels, _pathToModels.Capacity);
    }
}
