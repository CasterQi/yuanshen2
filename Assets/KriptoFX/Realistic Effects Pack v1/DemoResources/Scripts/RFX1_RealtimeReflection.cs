using UnityEngine;
using System.Collections;

public class RFX1_RealtimeReflection : MonoBehaviour
{
    ReflectionProbe probe;
    private Transform camT;

    void Awake()
    {
        probe = GetComponent<ReflectionProbe>();
        camT = Camera.main.transform;
    }

    void Update()
    {
        var pos = camT.position;
        probe.transform.position = new Vector3(
            pos.x,
            pos.y * -1,
            pos.z
        );
        probe.RenderProbe();
    }
}
