using UnityEngine;

[ExecuteInEditMode]
public class RFX4_LocalSpaceFix : MonoBehaviour
{
    void Update()
    {
#if UNITY_2017_1_OR_NEWER
        var invTransformMatrix = transform.worldToLocalMatrix;
#else
        var invTransformMatrix = Matrix4x4.identity;
#endif
        var ps = GetComponent<ParticleSystemRenderer>();
        if (ps != null)
        {
            if (Application.isPlaying)
                ps.material.SetMatrix("_InverseTransformMatrix", invTransformMatrix);
            else
                ps.sharedMaterial.SetMatrix("_InverseTransformMatrix", invTransformMatrix);
        }
    }
}
