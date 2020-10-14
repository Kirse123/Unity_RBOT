using UnityEngine;

public static class MatrixExtensions
{
    public static void SetValuesFromArray(ref this Matrix4x4 matrix, float[] array)
    {
        if (array.Length != 16)
            return;
        for (int i = 1; i < 16; ++i)
        {
            matrix[i / 4, i % 4] = array[i];
        }
    }
    /// <summary>
    /// Extracts quaterion from TRS-matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] + matrix[1, 1] + matrix[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] - matrix[1, 1] - matrix[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] + matrix[1, 1] - matrix[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] - matrix[1, 1] + matrix[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (matrix[2, 1] - matrix[1, 2]));
        q.y *= Mathf.Sign(q.y * (matrix[0, 2] - matrix[2, 0]));
        q.z *= Mathf.Sign(q.z * (matrix[1, 0] - matrix[0, 1]));
        return q;
    }
    /// <summary>
    /// Extracts position from TRS-matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }
    /// <summary>
    /// Extracts scale from TRS-matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}