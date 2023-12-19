using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class darkMode : MonoBehaviour
{
    //public Transform player;
    public static bool toDark=false;
    public Light Sun;
    public ReflectionProbe ReflectionProbe;
    public Light[] NightLights = new Light[0];
    public Material skybox_day;
    public Material skbox_dark;
    bool isDay=true;
    public GameObject water,seaFloor,mountain;
    MeshRenderer renderer_water,renderer_sea,renderer_mountain;
    private float startSunIntensity;
    private LightShadows startLightShadows;
    private Quaternion startSunRotation;
    private Color startAmbientLight;
    private float startAmbientIntencity;
    private float startReflectionIntencity;




    // Start is called before the first frame update
    void Start()
    {
        startSunIntensity = Sun.intensity;
        startLightShadows = Sun.shadows;
        startSunRotation = Sun.transform.rotation;
        startAmbientLight = RenderSettings.ambientLight;
        startAmbientIntencity = RenderSettings.ambientIntensity;
        startReflectionIntencity = RenderSettings.reflectionIntensity;
        renderer_water = water.GetComponent<MeshRenderer>();
        renderer_sea = seaFloor.GetComponent<MeshRenderer>();
        renderer_mountain = mountain.GetComponent<MeshRenderer>();



    }

    // Update is called once per frame
    void Update()
    {
        if(isDay==toDark){
            return;
        }else{
            switcher();
            
        }
        if (toDark)
        {
            ThiedPersonMovement.voiceState = 1;
            ThiedPersonMovement.voiceCD = 5;
            Debug.Log(isDay + "==" + toDark);
        }
        //if(Vector3.Distance(transform.position,player.position)<3){
        //        switcher();
        //}
    }

   


    public void switcher(){
        //isButtonPressed = true;
        RenderSettings.skybox = isDay ? skybox_day : skbox_dark;
        if (ReflectionProbe != null) ReflectionProbe.RenderProbe();
        Sun.intensity = !isDay ? 0.05f : startSunIntensity;
        Sun.shadows = isDay ? startLightShadows : LightShadows.None;
        foreach (var nightLight in NightLights)
        {
            nightLight.shadows = !isDay ? startLightShadows : LightShadows.None;
        }
        Sun.transform.rotation = isDay ? startSunRotation : Quaternion.Euler(350, 30, 90);
        RenderSettings.ambientLight = !isDay ? new Color(0.1f, 0.1f, 0.1f) : startAmbientLight;
        var lightInten = true ? 1 : 0.2f;
        RenderSettings.ambientIntensity = isDay ? startAmbientIntencity : lightInten;
        RenderSettings.reflectionIntensity = isDay ? startReflectionIntencity : 0.2f;
        renderer_water.enabled = isDay;
        renderer_sea.enabled = isDay;
        renderer_mountain.enabled = isDay;
        isDay = !isDay;
    }
}
