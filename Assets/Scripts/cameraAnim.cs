using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAnim : MonoBehaviour
{
	public Animator animator;
    //public Animation camear_play;
    float gameStartTime=0f;
    // Start is called before the	fr	update
    void Start()
    {
       // camear_play.Play();
    }

    // Update is called once per frame
    void Update()
    {
        gameStartTime += Time.deltaTime;
        if(gameStartTime>30f){
            animator.enabled = false;
        }
    }
}
