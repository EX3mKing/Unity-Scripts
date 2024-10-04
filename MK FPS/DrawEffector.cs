using UnityEditor;
using UnityEngine;

public class DrawEffector : MonoBehaviour
{
    public Mesh mesh;
    public Color color;
    public float size;
    public bool hide = false;
    public void OnDrawGizmos()
    {
        if (hide) return; 
        Gizmos.color = color;
        Gizmos.DrawMesh(mesh, this.transform.position, Quaternion.identity, Vector3.one * size);
    }
}
