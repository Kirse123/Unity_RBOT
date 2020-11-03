using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntrinsicSetup : MonoBehaviour
{
    private Camera _mainCamera;
    public float f = 35.0f; // f can be arbitrary, as long as sensor_size is resized to to make ax,ay consistient

    private void Awake()
    {
        _mainCamera = Camera.main;
        _mainCamera.usePhysicalProperties = true;
    }

    public void SetCameraParam(float[] K, int width, int height)
    {
        //ax, 0, x0, 0, ay, y0, 0, 0, 1.
        float ax, ay, sizeX, sizeY;
        float x0, y0, shiftX, shiftY;

        ax = K[0];
        ay = K[4];

        x0 = K[2];
        y0 = K[5];

        sizeX = f * width / ax;
        sizeY = f * height / ay;

        shiftX = -(x0 - width / 2.0f) / width;
        shiftY = (y0 - height / 2.0f) / height;

        _mainCamera.sensorSize = new Vector2(sizeX, sizeY);     // in mm, mx = 1000/x, my = 1000/y
        _mainCamera.focalLength = f;                            // in mm, ax = f * mx, ay = f * my
        _mainCamera.lensShift = new Vector2(shiftX, shiftY);    // W/2,H/w for (0,0), 1.0 shift in full W/H in image plane
    }
}
