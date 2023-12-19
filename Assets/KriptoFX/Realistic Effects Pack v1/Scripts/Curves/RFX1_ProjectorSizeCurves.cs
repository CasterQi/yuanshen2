using UnityEngine;
using System.Collections;

public class RFX1_ProjectorSizeCurves : MonoBehaviour
{
    public AnimationCurve ProjectorSize = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public bool IsLoop;

    private bool canUpdate;
    private float startTime;
    private Projector projector;

    private void Awake()
    {
        projector = GetComponent<Projector>();
        projector.orthographicSize = ProjectorSize.Evaluate(0);
    }

    private void OnEnable()
    {
        startTime = Time.time;
        canUpdate = true;
    }

    private void Update()
    {
        var time = Time.time - startTime;
        if (canUpdate) {
            var eval = ProjectorSize.Evaluate(time / GraphTimeMultiplier) * GraphIntensityMultiplier;
            projector.orthographicSize = eval;
        }
        if (time >= GraphTimeMultiplier) {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }
}