using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onDarkMode : MonoBehaviour
{
    GameObject controller;
    public Transform Player;
    public Transform ElephantSummoner;
    public Transform boss;
    public AK.Wwise.Event otherWorldTrigger;
    float timer=-1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   if(Vector3.Distance(transform.position, Player.position) < 20)
            {
            UI.textMark = 1;
            }
        if (Vector3.Distance(ElephantSummoner.position, Player.position) < 28)
        {
            UI.textMark = 3;
        }
        if (Vector3.Distance(boss.position, Player.position) < 30)
        {
            UI.textMark = 4;
        }

        if (Vector3.Distance(transform.position,Player.position)<2.3f&darkMode.toDark==false){
            darkMode.toDark = true;
            otherWorldTrigger.Post(gameObject);
            timer = 13f;
            //Invoke("textMarkChange", 12f);
        }
        if (timer > 0f & timer < 1)
        {
            UI.textMark = 2;

        }
        timer -= Time.deltaTime;
    }
    void textMarkChange()
    {
        UI.textMark = 2;
    }
}
