using RBOT_UnityPlugin;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ARCamera : MonoBehaviour
{
    private IFrameSource _frameSource;
    public ref IFrameSource GetFrameSource
    {
        get
        {
            return ref _frameSource;
        }
    }


    private bool _cameraIsAvailable = false;
    public bool CameraIsAvailable { get { return _cameraIsAvailable; }}

    [SerializeField]
    private bool _useCamera = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (_useCamera)
        {
            WebCamDevice[] cams = WebCamTexture.devices;
            if (cams.Length > 0)
            {
                WebCamTexture tmpWebCamTexture = new WebCamTexture(cams[0].name);
                _frameSource = new CameraFrame(tmpWebCamTexture);
                tmpWebCamTexture.Play();
                _cameraIsAvailable = true;
            }
        }
        else
        {
            _frameSource = new StaticImage(string.Concat(Application.dataPath, "/Resources/Images/frame.png"), 640, 512);
        }
    }

    private void OnPreRender()
    {
        Graphics.Blit(_frameSource.Texture, (RenderTexture)null);
    }

    public Color32[] GetFrameData()
    {
        return _frameSource.GetFrameData32();
    }
}
