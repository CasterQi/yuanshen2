using UnityEngine;
using System.Collections;

public class RFX1_AudioPitchCurves : MonoBehaviour
{
    public AnimationCurve AudioCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float GraphTimeMultiplier = 1;
    public float GraphPitchMultiplier = 1;
    public bool IsLoop;

    private bool canUpdate;
    private float startTime;
    private AudioSource audioSource;
    private float startPitch;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startPitch = audioSource.pitch;
        audioSource.pitch = AudioCurve.Evaluate(0) * GraphPitchMultiplier;
    }

    private void OnEnable()
    {
        startTime = Time.time;
        canUpdate = true;
        if(audioSource!=null ) audioSource.pitch = AudioCurve.Evaluate(0) * GraphPitchMultiplier;
    }

    private void Update()
    {
        var time = Time.time - startTime;
        if (canUpdate) {
            var eval = AudioCurve.Evaluate(time / GraphTimeMultiplier) * startPitch * GraphPitchMultiplier;
            audioSource.pitch = eval;
        }
        if (time >= GraphTimeMultiplier) {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }
}