using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class FogOverlay : MonoBehaviour
{
    public Material stencilFilterMaterial;

    private void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        var mr = GetComponent<MeshRenderer>();

        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Plane.fbx");
        }

        if (stencilFilterMaterial != null)
        {
            mr.material = stencilFilterMaterial;
        }

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;

        var col = GetComponent<Collider>();
        if (col != null) DestroyImmediate(col);
    }
}