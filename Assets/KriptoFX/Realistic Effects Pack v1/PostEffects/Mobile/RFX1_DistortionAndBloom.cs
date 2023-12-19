using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KriptoFX/RFX1_BloomAndDistortion")]
#if UNITY_5_4_OR_NEWER
//[ImageEffectAllowedInSceneView]
#endif
public class RFX1_DistortionAndBloom : MonoBehaviour
{
    [Range(0.05f, 1)]
    [Tooltip("Camera render texture resolution")]
    public float RenderTextureResolutoinFactor = 0.25f;

    public bool UseBloom = true;

    [Range(0.1f, 3)]
    [Tooltip("Filters out pixels under this level of brightness.")]
    public float Threshold = 2f;

    [SerializeField, Range(0, 1)]
    [Tooltip("Makes transition between under/over-threshold gradual.")]
    public float SoftKnee = 0f;

    [Range(1, 7)]
    [Tooltip("Changes extent of veiling effects in A screen resolution-independent fashion.")]
    public float Radius = 7;

    [Tooltip("Blend factor of the result image.")]
    public float Intensity = 1;

    [Tooltip("Controls filter quality and buffer resolution.")]
    public bool HighQuality;


    [Tooltip("Reduces flashing noise with an additional filter.")]
    public bool AntiFlicker;

    const string shaderName = "Hidden/KriptoFX/PostEffects/RFX1_Bloom";
    const string shaderAdditiveName = "Hidden/KriptoFX/PostEffects/RFX1_BloomAdditive";
    const string cameraName = "MobileCamera(Distort_Bloom_Depth)";

    RenderTexture source;
    RenderTexture depth;
    RenderTexture destination;
    private int previuosFrameWidth, previuosFrameHeight;
    private float previousScale;
    private Camera addCamera;
    private GameObject tempGO;
    private bool HDRSupported;

    private Material m_Material;

    public Material mat
    {
        get
        {
            if (m_Material == null)
                m_Material = CheckShaderAndCreateMaterial(Shader.Find(shaderName));

            return m_Material;
        }
    }

    private Material m_MaterialAdditive;

    public Material matAdditive
    {
        get
        {
            if (m_MaterialAdditive == null)
            {
                m_MaterialAdditive = CheckShaderAndCreateMaterial(Shader.Find(shaderAdditiveName));
                m_MaterialAdditive.renderQueue = 3900;
            }

            return m_MaterialAdditive;
        }
    }

    public static Material CheckShaderAndCreateMaterial(Shader s)
    {
        if (s == null || !s.isSupported)
            return null;

        var material = new Material(s);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }

    #region Private Members

    private const int kMaxIterations = 16;
    private readonly RenderTexture[] m_blurBuffer1 = new RenderTexture[kMaxIterations];
    private readonly RenderTexture[] m_blurBuffer2 = new RenderTexture[kMaxIterations];

    private void OnDisable()
    {
        if (m_Material != null)
            DestroyImmediate(m_Material);
        m_Material = null;

        if (m_MaterialAdditive != null)
            DestroyImmediate(m_MaterialAdditive);
        m_MaterialAdditive = null;

        if(tempGO != null)
            DestroyImmediate(tempGO);

        Shader.DisableKeyword("DISTORT_OFF");
        Shader.DisableKeyword("_MOBILEDEPTH_ON");
    }

    //private void OnGUI()
    //{
    //    if (Event.current.type.Equals(EventType.Repaint))
    //    {
    //        if (UseBloom && HDRSupported && destination != null) Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), destination, matAdditive);
    //    }
    //    GUI.Label(new Rect(250, 0, 30, 30), "HDR: " + HDRSupported, guiStyleHeader);
    //}
    public GUIStyle guiStyleHeader = new GUIStyle();

    void Start()
    {
        InitializeRenderTarget();
    }

    void LateUpdate()
    {
        if (previuosFrameWidth != Screen.width || previuosFrameHeight != Screen.height || Mathf.Abs(previousScale - RenderTextureResolutoinFactor) > 0.01f)
        {
            InitializeRenderTarget();
            previuosFrameWidth = Screen.width;
            previuosFrameHeight = Screen.height;
            previousScale = RenderTextureResolutoinFactor;
        }
        Shader.EnableKeyword("DISTORT_OFF");
        Shader.EnableKeyword("_MOBILEDEPTH_ON");
        GrabImage();
        if (UseBloom && HDRSupported) UpdateBloom();
        Shader.SetGlobalTexture("_GrabTexture", source);
        Shader.SetGlobalTexture("_CameraDepthTexture", depth);
        Shader.SetGlobalFloat("_GrabTextureScale", RenderTextureResolutoinFactor);
        Shader.DisableKeyword("DISTORT_OFF");
    }

    void OnPostRender()
    {
        Graphics.Blit(destination, null as RenderTexture, matAdditive);
    }

    private void InitializeRenderTarget()
    {
        var width = (int)(Screen.width * RenderTextureResolutoinFactor);
        var height = (int)(Screen.height * RenderTextureResolutoinFactor);
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float))
        {
            source = new RenderTexture(width, height, 0, RenderTextureFormat.RGB111110Float);
            depth = new RenderTexture(width, height, 8, RenderTextureFormat.Depth);
            HDRSupported = true;
            if (UseBloom) destination = new RenderTexture(RenderTextureResolutoinFactor > 0.99 ? width : width / 2,
                 RenderTextureResolutoinFactor > 0.99 ? height : height / 2, 0, RenderTextureFormat.RGB111110Float);
        }
        else
        {
            HDRSupported = false;
            source = new RenderTexture(width, height, 0, RenderTextureFormat.RGB565);
            depth = new RenderTexture(width, height, 8, RenderTextureFormat.Depth);
        }
    }

    private void UpdateBloom()
    {
        var useRGBM = Application.isMobilePlatform;
        if (source == null) return;
        // source texture size
        var tw = source.width;
        var th = source.height;

        // halve the texture size for the low quality mode
        if (!HighQuality)
        {
            tw /= 2;
            th /= 2;
        }

        // blur buffer format
        var rtFormat = useRGBM ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

        // determine the iteration count
        var logh = Mathf.Log(th, 2) + Radius - 8;
        var logh_i = (int)logh;
        var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

        // update the shader properties
        var threshold = Mathf.GammaToLinearSpace(Threshold);

        mat.SetFloat("_Threshold", threshold);

        var knee = threshold * SoftKnee + 1e-5f;
        var curve = new Vector3(threshold - knee, knee * 2, 0.25f / knee);
        mat.SetVector("_Curve", curve);

        var pfo = !HighQuality && AntiFlicker;
        mat.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

        mat.SetFloat("_SampleScale", 0.5f + logh - logh_i);
        mat.SetFloat("_Intensity", Mathf.Max(0.0f, Intensity));

        var prefiltered = RenderTexture.GetTemporary(tw, th, 0, rtFormat);

        Graphics.Blit(source, prefiltered, mat, AntiFlicker ? 1 : 0);

        // construct A mip pyramid
        var last = prefiltered;
        for (var level = 0; level < iterations; level++)
        {
            m_blurBuffer1[level] = RenderTexture.GetTemporary(last.width / 2, last.height / 2, 0, rtFormat);
            Graphics.Blit(last, m_blurBuffer1[level], mat, level == 0 ? (AntiFlicker ? 3 : 2) : 4);
            last = m_blurBuffer1[level];
        }

        // upsample and combine loop
        for (var level = iterations - 2; level >= 0; level--)
        {
            var basetex = m_blurBuffer1[level];
            mat.SetTexture("_BaseTex", basetex);
            m_blurBuffer2[level] = RenderTexture.GetTemporary(basetex.width, basetex.height, 0, rtFormat);
            Graphics.Blit(last, m_blurBuffer2[level], mat, HighQuality ? 6 : 5);
            last = m_blurBuffer2[level];
        }

        destination.DiscardContents();
        Graphics.Blit(last, destination, mat, HighQuality ? 8 : 7);


        for (var i = 0; i < kMaxIterations; i++)
        {
            if (m_blurBuffer1[i] != null) RenderTexture.ReleaseTemporary(m_blurBuffer1[i]);
            if (m_blurBuffer2[i] != null) RenderTexture.ReleaseTemporary(m_blurBuffer2[i]);
            m_blurBuffer1[i] = null;
            m_blurBuffer2[i] = null;
        }

        RenderTexture.ReleaseTemporary(prefiltered);
    }

    void GrabImage()
    {
        var cam = Camera.current;
        if (cam == null) cam = Camera.main;
       
        if (tempGO == null)
        {
            tempGO = new GameObject();
            tempGO.hideFlags = HideFlags.HideAndDontSave;
            tempGO.name = cameraName;
            addCamera = tempGO.AddComponent<Camera>();
            addCamera.enabled = false;
           // addCamera.transform.parent = cam.transform;
        }
        else addCamera = tempGO.GetComponent<Camera>();
        addCamera.CopyFrom(cam);
        addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        addCamera.depth--;
        //addCamera.targetTexture = source;
        
        addCamera.Render();


        //var cam = Camera.current;
        //if (cam != null && Camera.current.name == "SceneCamera")
        //{
        //    tempGO = GameObject.Find("MobileSceneCamera(Distort_Bloom_Depth)");
        //    if (tempGO == null)
        //    {
        //        tempGO = new GameObject();
        //        //tempGO.hideFlags = HideFlags.HideAndDontSave;
        //        tempGO.name = "MobileSceneCamera(Distort_Bloom_Depth)";
        //        addCamera = tempGO.AddComponent<Camera>();
        //        addCamera.enabled = false;

        //    }
        //    else addCamera = tempGO.GetComponent<Camera>();
        //    addCamera.CopyFrom(cam);
        //    addCamera.targetTexture = source;
        //    addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        //    addCamera.Render();
        //    return;
        //}


        //if (tempGO == null)
        //{
        //    tempGO = new GameObject();
        //    //tempGO.hideFlags = HideFlags.HideAndDontSave;
        //    tempGO.name = "MobileCamera(Distort_Bloom_Depth)";
        //    addCamera = tempGO.AddComponent<Camera>();
        //    addCamera.CopyFrom(Camera.main);
        //    addCamera.transform.parent = Camera.main.transform;
        //    addCamera.targetTexture = source;
        //    addCamera.enabled = false;
        //}
        //addCamera.SetTargetBuffers(source.colorBuffer, depth.depthBuffer);
        //addCamera.Render();

    }

    #endregion
}