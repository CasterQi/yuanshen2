using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject darkSwitcher;
    private float timer_gameStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer_gameStart += Time.deltaTime;
        if (timer_gameStart > 6 & timer_gameStart < 7)
        {
            darkMode.toDark = true;
        }
        if (timer_gameStart > 8 & timer_gameStart < 9)
        {
            darkMode.toDark = false;
        }
    }
}
