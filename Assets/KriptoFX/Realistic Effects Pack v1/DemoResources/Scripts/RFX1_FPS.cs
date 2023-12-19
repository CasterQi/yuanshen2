using UnityEngine;

public class RFX1_FPS : MonoBehaviour
{
    public GUIStyle guiStyleHeader = new GUIStyle();
    float timeleft;

    private float timeleft2;
    private const float updateTime = 0.5f;
    private float fps;
    private int frames; // Frames drawn over the interval

    #region Non-public methods

    //private void Awake()
    //{
    //  //guiStyleHeader.fontSize = 14;
    //  //guiStyleHeader.normal.textColor = new Color(1, 1, 1);
    //}

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 30, 30), "FPS: " + (int) fps / updateTime, guiStyleHeader);
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = frames;
            timeleft = updateTime;
            frames = 0;
        }
        
    }

    #endregion
}