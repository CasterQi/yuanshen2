using System;
using UnityEngine;

public class RFX1_DemoGUI : MonoBehaviour
{
    public int Current = 0;
	public GameObject[] Prefabs;
    public bool[] IsShield;
    public GameObject ShieldProjectile;
    public GameObject ShieldProjectile2;
    public float ShieldProjectileReactiovationTime = 7;
    public Light Sun;
    public ReflectionProbe ReflectionProbe;
    public Light[] NightLights = new Light[0];
    public Texture HUETexture;
    public bool UseMobileVersion;
    public GameObject MobileCharacter;
    public GameObject Target;
    public Color guiColor = Color.red;
    public RFX1_DistortionAndBloom RFX1_DistortionAndBloom;

    private int currentNomber;
	private GameObject currentInstance;
	private GUIStyle guiStyleHeader = new GUIStyle();
    private GUIStyle guiStyleHeaderMobile = new GUIStyle();
    float dpiScale;
    private bool isDay;
    private float colorHUE;
    private float startSunIntensity;
    private Quaternion startSunRotation;
    private Color startAmbientLight;
    private float startAmbientIntencity;
    private float startReflectionIntencity;
    private LightShadows startLightShadows;
    private float currentSpeed = 1;
    private GameObject mobileCharacterInstance;

	void Start () {
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;
        guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = guiColor;
        guiStyleHeaderMobile.fontSize = (int)(17f * dpiScale);

        ChangeCurrent(Current);
     
        startSunIntensity = Sun.intensity;
	    startSunRotation = Sun.transform.rotation;
	    startAmbientLight = RenderSettings.ambientLight;
	    startAmbientIntencity = RenderSettings.ambientIntensity;
	    startReflectionIntencity = RenderSettings.reflectionIntensity;
	    startLightShadows = Sun.shadows;

	    RFX1_DistortionAndBloom = Camera.main.GetComponent<RFX1_DistortionAndBloom>();

	}

    bool isButtonPressed;


    private void OnGUI()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            isButtonPressed = false;

        if (GUI.Button(new Rect(10*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "PREVIOUS EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(-1);
        }
        if (GUI.Button(new Rect(160*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "NEXT EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(+1);
        }
        var offset = 0f;
        //if (UseMobileVersion)
        //{
            
        //    offset += 50 * dpiScale;
        //    if (GUI.Button(new Rect(10*dpiScale, 63 * dpiScale, 285*dpiScale, 37*dpiScale), "ON / OFF REALISTIC BLOOM") ||
        //        (!isButtonPressed && Input.GetKeyDown(KeyCode.DownArrow)))
        //    {
        //        isUsedMobileBloom = !isUsedMobileBloom;
        //        RFX1_DistortionAndBloom.UseBloom = isUsedMobileBloom;
        //    }
        //    if(!isUsedMobileBloom) guiStyleHeaderMobile.normal.textColor = new Color(0.8f, 0.2f, 0.2f);
        //    else guiStyleHeaderMobile.normal.textColor = new Color(0.1f, 0.6f, 0.1f);
        //    GUI.Label(new Rect(400 * dpiScale, 15 * dpiScale, 100 * dpiScale, 20 * dpiScale), "Bloom is "+ (isUsedMobileBloom?"Enabled":"Disabled"), guiStyleHeaderMobile);
            
        //}
        if (GUI.Button(new Rect(10*dpiScale, 63*dpiScale + offset, 285*dpiScale, 37*dpiScale), "Day / Night") || (!isButtonPressed && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            isButtonPressed = true;
            if (ReflectionProbe != null) ReflectionProbe.RenderProbe();
            Sun.intensity = !isDay ? 0.05f : startSunIntensity;
            Sun.shadows = isDay ? startLightShadows : LightShadows.None;
            foreach (var nightLight in NightLights)
            {
                nightLight.shadows = !isDay ? startLightShadows : LightShadows.None;
            }
            Sun.transform.rotation = isDay ? startSunRotation : Quaternion.Euler(350, 30, 90);
            RenderSettings.ambientLight = !isDay ? new Color(0.1f, 0.1f, 0.1f) : startAmbientLight;
            var lightInten = !UseMobileVersion ? 1 : 0.2f;
            RenderSettings.ambientIntensity = isDay ? startAmbientIntencity : lightInten;
            RenderSettings.reflectionIntensity = isDay ? startReflectionIntencity : 0.2f;
            isDay = !isDay;
        }
      
        GUI.Label(new Rect(400*dpiScale, 15*dpiScale + offset / 2, 100*dpiScale, 20*dpiScale),
            "Prefab name is \"" + Prefabs[currentNomber].name +
            "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
        
        if (!IsShield[currentNomber] && !UseMobileVersion)
        {
            GUI.Label(new Rect(12 * dpiScale, 110 * dpiScale + offset, 50 * dpiScale, 20 * dpiScale), "Projectile Speed: " + Mathf.Round(currentSpeed * 10f) / 10f, guiStyleHeader);
            float oldCurrentSpeed = currentSpeed;
            if (!UseMobileVersion) currentSpeed = GUI.HorizontalSlider(new Rect(154 * dpiScale, 114 * dpiScale + offset, 135 * dpiScale, 15 * dpiScale), currentSpeed, 0.1f, 10);
           
            if (Math.Abs(oldCurrentSpeed - currentSpeed) > 0.001)
            {
                var animator = currentInstance.GetComponent<RFX1_AnimatorEvents>();
                if (animator != null)
                {
                    animator.Speed = currentSpeed;
                }
            }

            
        }

        GUI.DrawTexture(new Rect(12*dpiScale, 140*dpiScale + offset, 285*dpiScale, 15*dpiScale), HUETexture, ScaleMode.StretchToFill, false, 0);

        float oldColorHUE = colorHUE;
        colorHUE = GUI.HorizontalSlider(new Rect(12*dpiScale, 147*dpiScale + offset, 285*dpiScale, 15*dpiScale), colorHUE, 0, 360);


        if (Mathf.Abs(oldColorHUE - colorHUE) > 0.001)
        {
            //RFX4_ColorHelper.ChangeObjectColorByHUE(currentInstance, colorHUE / 360f);
            //var transformMotion = currentInstance.GetComponentInChildren<RFX4_TransformMotion>(true);
            //if (transformMotion != null)
            //{
            //    transformMotion.HUE = colorHUE / 360f;
            //    foreach (var collidedInstance in transformMotion.CollidedInstances)
            //    {
            //        if(collidedInstance!=null) RFX4_ColorHelper.ChangeObjectColorByHUE(collidedInstance, colorHUE / 360f);
            //    }
            //}

            var animator = currentInstance.GetComponent<RFX1_AnimatorEvents>();
            if (animator != null)
            {
                animator.HUE = colorHUE / 360f;
            }

            if (UseMobileVersion)
            {
                var settingColor = currentInstance.GetComponent<RFX1_EffectSettingColor>();
                if (settingColor == null) settingColor = currentInstance.AddComponent<RFX1_EffectSettingColor>();
                var hsv = RFX1_ColorHelper.ColorToHSV(settingColor.Color);
                hsv.H = colorHUE / 360f;
                settingColor.Color = RFX1_ColorHelper.HSVToColor(hsv);
            }

            //var rayCastCollision = currentInstance.GetComponentInChildren<RFX4_RaycastCollision>(true);
            //if (rayCastCollision != null)
            //{
            //    rayCastCollision.HUE = colorHUE / 360f;
            //    foreach (var collidedInstance in rayCastCollision.CollidedInstances)
            //    {
            //        if (collidedInstance != null) RFX4_ColorHelper.ChangeObjectColorByHUE(collidedInstance, colorHUE / 360f);
            //    }
            //}
        }
    }

    private GameObject instanceShieldProjectile;

    void ChangeCurrent(int delta)
    {
        currentSpeed = 1;
        currentNomber+=delta;
		if (currentNomber> Prefabs.Length - 1)
			currentNomber = 0;
		else if (currentNomber < 0)
			currentNomber = Prefabs.Length - 1;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            RemoveClones();
        }

        currentInstance = Instantiate(Prefabs[currentNomber]);
        var targetScript = currentInstance.GetComponent<RFX1_AnimatorEvents>();
        if(targetScript != null) targetScript.Target = Target;

        var targetScript2 = currentInstance.GetComponent<RFX1_Target>();
        if (targetScript2 != null) targetScript2.Target = Target;

        CancelInvoke("ReactivateShieldProjectile");
        if (IsShield[currentNomber])
        {
            if(currentNomber != 23)
            InvokeRepeating("ReactivateShieldProjectile", 5, ShieldProjectileReactiovationTime);
            else InvokeRepeating("ReactivateShieldProjectile", 3, 3);
        }
        var transformMotion = currentInstance.GetComponentInChildren<RFX1_TransformMotion>();
        if (transformMotion != null) currentSpeed = transformMotion.Speed;

        if (UseMobileVersion)
        {
            CancelInvoke("ReactivateEffect");
            transformMotion = currentInstance.GetComponentInChildren<RFX1_TransformMotion>();
            if (transformMotion != null)
                transformMotion.CollisionEnter += (sender, info) => { Invoke("ReactivateEffect", 3); };

        }
        if (mobileCharacterInstance != null)
        {
            Destroy(mobileCharacterInstance);
        }
        if (IsShield[currentNomber] && UseMobileVersion) mobileCharacterInstance = Instantiate(MobileCharacter);
    }

    

    void RemoveClones()
    {
        var allGO = FindObjectsOfType<GameObject>();
        foreach (var go in allGO)
        {
            if(go.name.Contains("(Clone)")) Destroy(go);
        }
    }

    void ReactivateShieldProjectile()
    {
        
        if (instanceShieldProjectile != null) Destroy(instanceShieldProjectile);
        instanceShieldProjectile = (currentNomber != 23)
            ? Instantiate(ShieldProjectile)
            : Instantiate(ShieldProjectile2);
        instanceShieldProjectile.SetActive(false);
        instanceShieldProjectile.SetActive(true);
    }

    void ReactivateEffect()
    {
        currentInstance.SetActive(false);
        currentInstance.SetActive(true);
    }
}
