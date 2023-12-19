using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static int textMark = 0;
    bool textMark2 = false;
    public bool isboomX = false;
    public CanvasGroup canvasGroup;
    public Canvas dialogCanvas;
    public GameObject dialog2Canvas;
    public GameObject dialog2Ani;
    float timer_gameStart;
    bool dialogState=false;
    bool dialog2State=false;
    public AK.Wwise.Event[] Events;
    private Animation dialogAnimation;
    private Animation dialog2Animation;
    string[] text;
    string[] text2;
    bool isTextTriggered = false;
    bool isTextTriggered2 = false;
    bool isTextTriggered3 = false;
    bool isTextTriggered4 = false;
    bool isTextTriggered5 = false;
    bool isTextTriggered6 = false;
    bool isTextTriggered7 = false;
    Text text1;
    public GameObject boss;
    public AK.Wwise.Event e;
    public AK.Wwise.Event e2;
    public GameObject txt1;
    // Start is called before the first frame update
    void Start()
    {
        dialogAnimation = dialogCanvas.GetComponent<Animation>();
        dialog2Animation = dialog2Ani.GetComponent<Animation>();
        //text = new string[10] { };
        text2 = new string[10];
        text2.SetValue("尝试找出这个世界的异常", 0);
        dialog2Canvas.GetComponent<Text>().text = text2[0];
        text1 = GameObject.Find("Text001").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (textMark == 1&isTextTriggered==false)
        {
            dialogX();
            text1.text = "这个世界以前的主人看起来非常喜欢古代地球的自然景色呢，派蒙也很喜欢！但是，这个看起来像魔法阵一样的东西。。。好奇怪,我们要不去看看！";
            text2[0] = "探索奇怪的魔法阵";
            isTextTriggered = true;

        }
        else if (textMark == 2&isTextTriggered2==false)
        {
            dialogX();
            text1.text = "啊？太阳呢？我就说这个魔法阵有问题嘛！小心丛林里的怪物！";
            text2[0] = "探索黑暗的丛林";
            isTextTriggered2 = true;
        }
        else if (textMark == 3 & isTextTriggered3 == false)
        {
            dialogX();
            text1.text = "在丛林里面栖息的动物，怎么都变成了这个样子。啊！大象！";
            text2[0] = "尝试找出丛林异常的原因";
            isTextTriggered3 = true;
        }
        else if(textMark == 4 & isTextTriggered4 == false)
        {
            dialogX();
            text1.text = "让我看看！那是这个世界的管理时间的进程，<b><i>进程：时间</i></b>发生了故障，我们必须击败它，然后安全结束这个世界";
            text2[0] = "击败<b><i>进程：时间</i></b>";
            isTextTriggered4 = true;
        }
        else if ((textMark == 5 |isboomX)& isTextTriggered5 == false)
        {
            dialogX();
            text1.text = "<b><i>进程：时间</i></b>可以随意操控昼夜，小心那些被它篡改的动物！";
            text2[0] = "击败<b><i>进程：时间</i></b>";
            isTextTriggered5 = true;
        }
        else if ((textMark == 6|textMark2 ) & isTextTriggered6 == false)
        {
            dialogX();
            text1.text = "这个世界终于恢复正常了，灾祸之前的地球这么漂亮嘛，真可惜！我们向父进程汇报，继续下一个任务吧！";
            text2[0] = "通关！感谢游玩";
            isTextTriggered6 = true;
        }

        if (timer_gameStart > 26&timer_gameStart<30)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, 1 * Time.deltaTime);

        }else
        if (timer_gameStart > 30&timer_gameStart<35)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, 1 * Time.deltaTime);

        }
        if (timer_gameStart > 38 & timer_gameStart < 40 & dialogState==false)
        {
            dialog();
        }

        if (!boss&isTextTriggered7==false)
        {
            txt1.SetActive(true);
            isTextTriggered7 = true;

            e.Post(gameObject);
            e2.Post(gameObject);
            Invoke("NewMethod", 10f);
        }

        timer_gameStart += Time.deltaTime;
    }

    private void NewMethod()
    {
        txt1.SetActive(false);
        textMark = 6;
        textMark2 = true;
    }

    private void dialogX()
    {
        dialog2Close();
        
        dialogAnimation["对话框"].speed = 1f;
        dialogAnimation.Play("对话框");
        Events[0].Post(gameObject);
        Invoke("dialogClose", 8);
    }

    private void dialog(string text = "",float time = 15f)
    {
        dialogAnimation.Play("对话框");
        dialogState = true;
        Events[0].Post(gameObject);
        Invoke("dialogClose", time);
    }
    private void dialogClose()
    {
        if (dialog2State == true)
        {
            dialog2Close();
        }
        dialogAnimation["对话框"].time = dialogAnimation["对话框"].clip.length;
        dialogAnimation["对话框"].speed = -1f;
        dialogAnimation.Play("对话框");

        dialogState = false;
        Invoke("dialog2", 2f);
    }

    private void dialog2()
    {
        dialog2Animation["对话框22"].speed = 1f;
        dialog2Animation.Play("对话框22");
        dialog2State = true;
        dialog2Canvas.GetComponent<Text>().text = text2[0];
        Events[1].Post(gameObject);

        //Invoke("dialogClose", time);
    }
    private void dialog2Close()
    {
        
        dialog2Animation["对话框22"].time = dialog2Animation["对话框22"].clip.length;
        dialog2Animation["对话框22"].speed = -1f;
        dialog2Animation.Play("对话框22");
        dialog2State = false;
    }
}
