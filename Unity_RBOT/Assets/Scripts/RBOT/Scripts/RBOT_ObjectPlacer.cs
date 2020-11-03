using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RBOT_UnityPlugin;

public class RBOT_ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject objectPrefab;

    private List<RBOT_ModelScript> objects;
    private Camera _mainCamera;

    private void Awake()
    {
        Controller.ObjectAddedEvent += OnObjectAdded;
        Controller.ObjectRomevedEvent += OnObjectRemoved;
        Controller.PoseEstimationEvent += OnPosesEstimated;
        objects = new List<RBOT_ModelScript>();
        _mainCamera = Camera.main;
    }

    private void OnObjectAdded(RBOT_ObjectAddedEventArgs e)
    {
        if (e.Result == ResultCode.Successful)
        {
            // Get initial object's pose
            Vector3 pos = new Vector3(e.object3D.tx, e.object3D.ty, e.object3D.tz);
            Quaternion rot = Quaternion.Euler(e.object3D.alpha, e.object3D.beta, e.object3D.gamma);

            GameObject tmpObj = Instantiate(objectPrefab, _mainCamera.transform);
            tmpObj.transform.localPosition = pos;
            tmpObj.transform.localRotation = rot;

            RBOT_ModelScript modelScript = tmpObj.GetComponent<RBOT_ModelScript>();
            
            // Load model
            string tmpStr = System.IO.Path.GetFileName(e.object3D.FullFileName);
            modelScript.LoadModel(tmpStr);            
            
            // Save ref
            objects.Add(modelScript);
        }
    }
    private void OnObjectRemoved(RBOT_ObjectRemovedEventArgs e)
    {
        if (e.Result == ResultCode.Successful)
        {
            Destroy(objects[e.Index].gameObject);
            objects.RemoveAt(e.Index);
        }
    }
    private void OnPosesEstimated(RBOT_PoseEstimationEventArgs e)
    {
        if (e.Result == ResultCode.Successful)
        {
            for (int i = 0; i < this.objects.Count; ++i)
                this.objects[i].SetPose(Controller.Instance.Objects[i].TRS);
        }
    }
}
