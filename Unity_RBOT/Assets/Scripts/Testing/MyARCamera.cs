using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyARCamera : MonoBehaviour
{
    private WebCamTexture _webCamTexture;
    public WebCamTexture WebCam { get { return _webCamTexture; } }

    private Texture2D _testImage;

    private bool _cameraIsAvailable = false;
    public bool CameraIsAvailable { get { return _cameraIsAvailable; }}

    [SerializeField]
    private bool _useCamera = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_useCamera)
        {
            WebCamDevice[] cams = WebCamTexture.devices;
            if (cams.Length > 0)
            {
                _webCamTexture = new WebCamTexture(cams[0].name);
                _webCamTexture.Play();
                _cameraIsAvailable = true;
            }
        }
        else
        {
            byte[] imgDate = File.ReadAllBytes(string.Concat(Application.dataPath, "/Resources/Images/frame.png"));
            _testImage = new Texture2D(640, 512);
            _testImage.LoadImage(imgDate);
        }
    }

    private void OnPreRender()
    {
        if (_cameraIsAvailable)
        {
            Graphics.Blit(_webCamTexture, (RenderTexture)null);
        }
        else if (!_useCamera)
        {
            Graphics.Blit(_testImage, (RenderTexture)null);
        }
    }
}
