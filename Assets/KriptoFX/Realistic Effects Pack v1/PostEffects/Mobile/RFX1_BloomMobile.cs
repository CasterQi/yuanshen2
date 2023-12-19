using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFX1_BloomMobile : MonoBehaviour {

    public Shader shader;
    public RenderTexture rt;
    public RenderTexture rt2;
    Camera depthCamera;
    Camera bloomCamera;

    // Use this for initialization
    void Awake () {
        rt = new RenderTexture(Screen.width/2, Screen.height/2, 16, RenderTextureFormat.Depth);
        //rt2 = new RenderTexture(Screen.width/2, Screen.height/2, 0, RenderTextureFormat.RGB565);
        var oldCam = GetComponent<Camera>();
        //oldCam.cullingMask &= ~(1 << LayerMask.NameToLayer("BloomMobileEffect"));
        //oldCam.depthTextureMode = DepthTextureMode.Depth;

        var go = new GameObject("DepthCamera");
        depthCamera = go.AddComponent<Camera>();
        depthCamera.CopyFrom(oldCam);
        depthCamera.targetTexture = rt;
        depthCamera.SetTargetBuffers(rt.colorBuffer, rt.depthBuffer);
        depthCamera.SetReplacementShader(shader, "RenderType");
        depthCamera.clearFlags = CameraClearFlags.SolidColor;
        if( SystemInfo.usesReversedZBuffer) depthCamera.backgroundColor = Color.black;
        else depthCamera.backgroundColor = Color.white;
        depthCamera.transform.localPosition = Vector3.zero;
        depthCamera.transform.rotation = new Quaternion();
        depthCamera.transform.parent = oldCam.transform;
        depthCamera.enabled = false;

        //var go2 = new GameObject("BloomCamera");
        //bloomCamera = go2.AddComponent<Camera>();
        //bloomCamera.targetTexture = rt2;
        //bloomCamera.cullingMask = (1 << LayerMask.NameToLayer("BloomMobileEffect"));
        ////bloomCamera.SetReplacementShader(shader, "RenderType");
        //bloomCamera.clearFlags = CameraClearFlags.SolidColor;
        //bloomCamera.backgroundColor = Color.black;
        //bloomCamera.transform.localPosition = Vector3.zero;
        //bloomCamera.transform.rotation = new Quaternion();
        //bloomCamera.transform.parent = oldCam.transform;
        //bloomCamera.enabled = false;

        // target = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
        // targetDepth = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
      
        //bloomCamera.SetTargetBuffers(rt2.colorBuffer, rt.depthBuffer);

        Shader.SetGlobalTexture("_CameraDepthTextureSelf", rt);
        Shader.SetGlobalTexture("_BloomTextureSelf", rt2);
        Shader.EnableKeyword("_MobileDepth_ON");
        
        Shader.SetGlobalFloat("_BloomThreshold", 1);
        Shader.SetGlobalVector("test", vec);
    }
    public Vector4 vec = new Vector4(1,0,1,0);

    private void OnDisable()
    {
        Shader.DisableKeyword("_MobileDepth_ON");
    }

    // Update is called once per frame
    void OnPreRender()
    {
        Shader.SetGlobalVector("test", vec);
        depthCamera.transform.localPosition = Vector3.zero;
        depthCamera.transform.rotation = new Quaternion();
        depthCamera.Render();

        //bloomCamera.transform.localPosition = Vector3.zero;
        //bloomCamera.transform.rotation = new Quaternion();
        //Shader.EnableKeyword("_MobileBloom_ON");
        //bloomCamera.Render();
        //Shader.DisableKeyword("_MobileBloom_ON");
        //var oldCam = GetComponent<Camera>();

        //depthCamera.CopyFrom(oldCam);
        //depthCamera.targetTexture = rt;

        //depthCamera.SetReplacementShader(shader, "RenderType");

        //// |= (1 << LayerMask.NameToLayer("BloomMobileEffect"));
        ////cam.cullingMask = (1 << LayerMask.NameToLayer("BloomMobileEffect"));
        //depthCamera.clearFlags = CameraClearFlags.SolidColor;
        //depthCamera.backgroundColor = Color.black;
        //depthCamera.depth -= depthCamera.depth;


    }
}
