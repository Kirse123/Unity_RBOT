using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject modelPrefab;
    private List<Vector4> rMat;
    private Vector3 tranformVect;

    // Start is called before the first frame update
    void Start()
    {
        
        List<Vector4> rowList = new List<Vector4>();

        rowList.Add(new Vector4(-0.85165083f, 0.011513352f, -0.52398312f, 0f));
        rowList.Add(new Vector4(0.39713106f, -0.63824002f, -0.65949702f, 0f));
        rowList.Add(new Vector4(-0.34202012f, -0.76975113f, 0.53898555f, 0f));
        rowList.Add(new Vector4(15f, -35f, 515f, 1f));

        //====Test rotate 30 around Y=================================
        //[0.86602539, 0, 0.5, 0;
        // 0, 1, 0, 0;
        //-0.5, 0, 0.86602539, 0;
        // 0, 0, 0, 1]
        //rowList.Add(new Vector4(0.86602539f, 0f, -0.5f,  0f));
        //rowList.Add(new Vector4(0f, 1f, 0f,  0f));
        //rowList.Add(new Vector4(0.5f, 0f, 0.86602539f,  0f));
        //rowList.Add(new Vector4(15f, -35f, 515f,  1f));

        //=================random (248, -78, 31)================================
        //rowList.Add(new Vector4(0.17821507f, 0.58444804f, -0.79161859f,  0f));
        //rowList.Add(new Vector4(-0.10708243f, -0.78820014f, -0.60603142f,  0f));
        //rowList.Add(new Vector4(-0.97814763f, 0.19277236f, -0.077885024f,  0f));
        //rowList.Add(new Vector4(15f, -35f, 515f,  1f));
        //========================================================================
        //======================90 x=============================
        //rowList.Add(new Vector4(0.99999994f, 0f, 0f,  0f));
        //rowList.Add(new Vector4(0f, -4.3711388e-08f, 1f,  0f));
        //rowList.Add(new Vector4(0f, -1f, -4.3711388e-08f,  0f));
        //rowList.Add(new Vector4(15f, -35f, 515f,  1f));
        //=====================================================
        //======================90 y==========================
        //rowList.Add(new Vector4(-4.3711388e-08f, 0f, -1f, 0f));
        //rowList.Add(new Vector4(0f, 0.99999994f, 0f, 0f));
        //rowList.Add(new Vector4(1f, 0f, -4.3711388e-08f, 0f));
        //rowList.Add(new Vector4(15f, -35f, 515f, 1f));
        //======================90 z==========================
        //rowList.Add(new Vector4(-4.3711388e-08f, 1f, 0f, 0f));
        //rowList.Add(new Vector4(-1f, -4.3711388e-08f, 0f, 0f));
        //rowList.Add(new Vector4(0f, 0f, 0.99999994f, 0f));
        //rowList.Add(new Vector4(15f, -35f, 515f, 1f));

        //rowList.Add(new Vector4(1f, 0f, 0f, 0f));
        //rowList.Add(new Vector4(0f, 1f, 0f, 0f));
        //rowList.Add(new Vector4(0f, 0f, 1f, 0f));
        //rowList.Add(new Vector4(15f, -35f, 515f, 1f));
        //============================================================

        Matrix4x4 trsMat = new Matrix4x4(rowList[0], rowList[1], rowList[2], rowList[3]);
        
        //=================================================================================================
        //F = [1  0  0  0]
        //    [0 -1  0  0]
        //    [0  0  1  0]
        //    [0  0  0  1] 
        Matrix4x4 F = new Matrix4x4(new Vector4(  1,  0,  0, 0), 
                                    new Vector4(  0, -1,  0, 0), 
                                    new Vector4(  0,  0,  1, 0), 
                                    new Vector4(  0,  0,  0, 1));
        trsMat = F * trsMat * F.inverse;
        //================================================================================================

        GameObject placedModel = Instantiate(modelPrefab, Camera.main.transform);
        placedModel.transform.localPosition = trsMat.ExtractPosition();
        placedModel.transform.localRotation = trsMat.ExtractRotation();
        placedModel.transform.localScale = trsMat.ExtractScale();
        
        //placedModel.transform.localScale = new Vector3(-placedModel.transform.localScale.x,
        //                                               -placedModel.transform.localScale.y,
        //                                               placedModel.transform.localScale.z);

        //==============================================Debug output=========================================================
        /*
        Quaternion tmpQuaternion = trsMat.ExtractRotation();

        Debug.LogFormat("0) x = {0}, y = {1}, z = {2}, w = {3}", tmpQuaternion.x, tmpQuaternion.y, tmpQuaternion.z, tmpQuaternion.w);
        Debug.LogFormat("1) alpha = {0}, beta = {1}, gamma = {2}", tmpQuaternion.eulerAngles.x,
                                                                tmpQuaternion.eulerAngles.y,
                                                                tmpQuaternion.eulerAngles.z);

        tmpQuaternion = QuaternionFromMatrix(trsMat);
        Debug.LogFormat("2) alpha = {0}, beta = {1}, gamma = {2}", tmpQuaternion.eulerAngles.x,
                                                        tmpQuaternion.eulerAngles.y,
                                                        tmpQuaternion.eulerAngles.z);

        Matrix4x4 testRot = Matrix4x4.Rotate(tmpQuaternion);
        Debug.Log(testRot);

        Debug.Log(Matrix4x4.Rotate(Quaternion.Euler(55, -35, 205)));
        Debug.LogFormat(" cameraToWorldMatrix {0}", Camera.main.cameraToWorldMatrix);
        ====================================================================================================================*/

        Debug.Log(trsMat);
    }
}
