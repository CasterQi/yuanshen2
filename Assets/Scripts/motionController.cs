using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class motionController : MonoBehaviour
{

    public static Animator animator;

    private void Start()
    {
        animator = (UnityEngine.Animator)GetComponent("Animator");
        animator.SetInteger("animation", 20);
    }
    private void Update()
    {
        
    }

    public static void setAnimation(int i){
        animator.SetInteger("animation", i);
       // Invoke("",5);
    }
    private static void SetStopAnimation(){
        animator.SetInteger("animation", 1);
    }
    //public static int getParamaeter1(string s){
    //    return (int) animator.GetParameter(0);
    //}



    //public void SwapVisibility(GameObject obj)
    //{
    //    obj.SetActive(!obj.activeSelf);
    //}


    //public void SetFloat(string parameter = "key,value")
    //{
    //    char[] separator = { ',', ';' };
    //    string[] param = parameter.Split(separator);

    //    string name = param[0];
    //    float value = (float)Convert.ToDouble(param[1]);

    //    Debug.Log(name + " " + value);

    //    foreach (Animator a in animators)
    //    {
    //        a.SetFloat(name, value);
    //    }
    //}
    //public void SetInt(string parameter = "key,value")
    //{
    //    char[] separator = { ',', ';' };
    //    string[] param = parameter.Split(separator);

    //    string name = param[0];
    //    int value = Convert.ToInt32(param[1]);

    //    Debug.Log(name + " " + value);

    //    foreach (Animator a in animators)
    //    {
    //        a.SetInteger(name, value);
    //    }
    //}

    //public void SetBool(string parameter = "key,value")
    //{
    //    char[] separator = { ',', ';' };
    //    string[] param = parameter.Split(separator);

    //    string name = param[0];
    //    bool value = Convert.ToBoolean(param[1]);

    //    Debug.Log(name + " " + value);

    //    foreach (Animator a in animators)
    //    {
    //        a.SetBool(name, value);
    //    }
    //}

    //public void SetTrigger(string parameter = "key,value")
    //{
    //    char[] separator = { ',', ';' };
    //    string[] param = parameter.Split(separator);

    //    string name = param[0];

    //    Debug.Log(name);

    //    foreach (Animator a in animators)
    //    {
    //        a.SetTrigger(name);
    //    }
    //}
}
