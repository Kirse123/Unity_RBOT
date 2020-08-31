using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBOT_Model : MonoBehaviour
{
    //reference to model's mesh filter
    [SerializeField]
    private MeshFilter meshFilter;
    
    //reference to model's mesh
    private Mesh _mesh;
    public Mesh Mesh { get { return _mesh; } }

    private bool _isFlipped = false;
    public bool IsFliipped { get { return _isFlipped; } }

    //describes axises for model flipping
    public bool flipX = true;
    public bool flipY = true;
    public bool flipZ;

    private void Awake()
    {
        if (meshFilter != null)
            _mesh = meshFilter.mesh;
    }

    private void Start()
    {
        if (!this.IsFliipped)
            Flip(_mesh);
    }

    /// <summary>
    /// Flipps model's mesh reletive to flipX, flipY, flipZ variables
    /// </summary>
    /// <param name="mesh"></param>
    void Flip(Mesh mesh)
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
        if (flipX ^ flipY ^ flipZ) FlipNormals(mesh);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        Debug.Log("Flipped");
    }
    void FlipNormals(Mesh mesh)
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

