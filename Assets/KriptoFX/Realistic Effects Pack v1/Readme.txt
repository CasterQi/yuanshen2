Pack includes prefabs of main effects + prefabs of collision effects (Assets\KriptoFX\Realistic Effects Pack v1\Prefabs\). 


Support platforms:

PC/Consoles/VR/Mobiles
All effects tested on Oculus Rift CV1 with single and dual mode rendering and work perfect. 
------------------------------------------------------------------------------------------------------------------------------------------

NOTE:
For correct work on PC in your project scene you need:

1) Download unity free posteffects 
https://assetstore.unity.com/packages/essentials/post-processing-stack-83912
2) Add "PostProcessingBehaviour.cs" on main Camera.
3) Set the "PostEffects" profile. (path "Assets\KriptoFX\Realistic Effects Pack v1\PostEffects.asset")
4) You should turn on "HDR" on main camera for correct posteffects. 
If you have forward rendering path (by default in Unity), you need disable antialiasing "edit->project settings->quality->antialiasing"
or turn of "MSAA" on main camera, because HDR does not works with msaa. If you want to use HDR and MSAA then use "post effect msaa". 

For correct work on MOBILES in your project scene you need:

1) Add script "RFX1_DistortionAndBloom.cs" on main camera. It's allow you to see correct distortion, soft particles and physical bloom 
The mobile bloom posteffect work if mobiles supported HDR textures or supported openGLES 3.0

------------------------------------------------------------------------------------------------------------------------------------------

Using effects:

Just drag and drop prefab of effect on scene and use that :)
If you want use effects in runtime, use follow code:

"Instantiate(prefabEffect, position, rotation);"

Using projectile collision event:
void Start ()
{
	var tm = GetComponentInChildren<RFX1_TransformMotion>(true);
	if (tm!=null) tm.CollisionEnter += Tm_CollisionEnter;
}

private void Tm_CollisionEnter(object sender, RFX1_TransformMotion.RFX1_CollisionInfo e)
{
        Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
}

Using shield interaction:
You need add script "RFX1_ShieldInteraction" on projectiles which should react on shields.

------------------------------------------------------------------------------------------------------------------------------------------

Effect modification:

For scaling just change "transform" scale of effect. 
All effects includes helpers scripts (collision behaviour, light/shader animation etc) for work out of box. 
Also you can add additional scripts for easy change of base effects settings. Just add follow scripts to prefab of effect.
 
RFX1_EffectSettingColor - for change color of effect (uses HUE color). Can be added on any effect.
RFX1_EffectSettingProjectile - for change projectile fly distance, speed and collided layers. 
RFX1_EffectSettingVisible - for change visible status of effect using smooth fading by time. 
RFX1_Target - for homing move to target. 

