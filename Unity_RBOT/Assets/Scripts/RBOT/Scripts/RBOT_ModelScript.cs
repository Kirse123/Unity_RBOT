using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBOT_ModelScript : MonoBehaviour
{
    // Matrix, used for coordinate system transformation
    public static readonly Matrix4x4 F = new Matrix4x4(new Vector4(1,  0, 0, 0),
                                                       new Vector4(0, -1, 0, 0),
                                                       new Vector4(0,  0, 1, 0),
                                                       new Vector4(0,  0, 0, 1));

    public GameObject Model;
    
    //reference to model's mesh
    private Mesh _mesh;
    public Mesh Mesh { get { return _mesh; } }

    private bool _isFlipped = false;
    public bool IsFliipped { get { return _isFlipped; } }

    //describes axises for model flipping
    public bool flipX = true;
    public bool flipY = true;
    public bool flipZ;

    /// <summary>
    /// Load model from Assets/Resources/Models/
    /// </summary>
    /// <param name="modelName">Model name to load</param>
    public void LoadModel(string modelNameWithExtension)
    {
        // Rename gameObject 
        gameObject.name = System.IO.Path.GetFileNameWithoutExtension(modelNameWithExtension);

        // Place model
        Model = (GameObject)Instantiate(Resources.Load(System.IO.Path.Combine("Prefabs", gameObject.name)), gameObject.transform);
        Model.transform.localPosition = Vector3.zero;
        Model.transform.localRotation = Quaternion.identity;
        // Save ref to mesh
        _mesh = Model.GetComponentInChildren<MeshFilter>().mesh;

        if (!this.IsFliipped)
            Flip(_mesh);
    }

    /// <summary>
    /// Set pose to representing model
    /// </summary>
    /// <param name="TRS">Transform-Ratation-Scale Matrix</param>
    public void SetPose(Matrix4x4 TRS)
    {

        /*
        Matrix4x4 _trs = new Matrix4x4(new Vector4(-0.85165f, 0.01151f, -0.52398f, 0f),
                                       new Vector4(0.39713f, -0.63824f, -0.65950f, 0f),
                                       new Vector4(-0.34202f, -0.76975f, 0.53899f, 0),
                                       new Vector4(15, -35, 515, 1));
        */

        Matrix4x4 _trs = F * TRS * F.inverse;
        gameObject.transform.localPosition = _trs.ExtractPosition();
        gameObject.transform.localRotation = _trs.ExtractRotation();
        gameObject.transform.localScale = _trs.ExtractScale();
    }

    /// <summary>
    /// Flipps model's mesh reletive to flipX, flipY, flipZ variables
    /// </summary>
    /// <param name="mesh"></param>
    private void Flip(Mesh mesh)
    {
        if (mesh == null) return;
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 c = verts[i];
            if (flipX) c.x *= -1;
            if (flipY) c.y *= -1;
            if (flipZ) c.z *= -1;
            verts[i] = c;
        }

        mesh.vertices = verts;
        if (flipX ^ flipY ^ flipZ)
            FlipNormals(mesh);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        Debug.Log("Flipped");
    }
    private void FlipNormals(Mesh mesh)
    {
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length / 3; i++)
        {
            int a = tris[i * 3 + 0];
            int b = tris[i * 3 + 1];
            int c = tris[i * 3 + 2];
            tris[i * 3 + 0] = c;
            tris[i * 3 + 1] = b;
            tris[i * 3 + 2] = a;
        }
        mesh.triangles = tris;

        Debug.Log("Flipped Normals");
    }
}

