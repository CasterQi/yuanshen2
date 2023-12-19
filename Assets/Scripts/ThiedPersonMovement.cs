using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiedPersonMovement : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;
    public GameObject GroundCheck;
    public LayerMask GroundMask,GrassMask,WaterMask;
    public Transform sword;
    public Transform Player;
    public GameObject[] prefabEffect;
    public AK.Wwise.Switch footStepMud;
    public AK.Wwise.Switch footStepWater;
    public AK.Wwise.Switch footStepGrass;
    public AK.Wwise.Event Event;//脚步
    public AK.Wwise.Event EventAttack;
    public AK.Wwise.Event EventVoice1;
    public AK.Wwise.Event EventVoice2;
    public AK.Wwise.Event EventVoice3;
    public AK.Wwise.Event EventSword;
    public AK.Wwise.Event EventBGM;
    public AK.Wwise.Event EventBGMin;
    public AK.Wwise.Event EventWhoosh;
    public static int voiceState = 0;
    public static float voiceCD =33f;
    public GameObject[] swordPrefabs;


    float timer_gameStart = 0;
    float smoothVelocity;

    float smoothTime = 0.1f;
    float speed = 5f;
    float gravity = -9.8f;
    float checkerRadius = 0.4f;
    bool isGrounded;
    bool m_cursorIsLocked;
    Vector3 moveDir;
    float timer_action,timer_attack,timer_jump;
    int attack_num = 0;
    public static  bool  isGrass,isWater;
    float footStepCD = 0;
    int voice1SayedNum = 0;
    bool isBGMPosted = false;
    bool isBGMinPosted = false;
    float dodgeSpeedCD = 0.35f;
    float dodgeCD = 1f;
    public TrailRenderer trailRenderer;
    bool isWhooshPlayed = false;
    Vector3 velocity;
    GameObject swordLight;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   //音效控制
        if (timer_gameStart > 1 & !isBGMinPosted) { EventBGMin.Post(gameObject);isBGMinPosted = true; }
        if (timer_gameStart > 35&!isBGMPosted)
        {
            EventBGM.Post(gameObject);
            isBGMPosted = true;
        }
        if (voiceState == 1) { EventBGM.Stop(gameObject); }
        if (voiceCD < 0 & timer_attack < -3)
        {
            if (voiceState == 0)
            {
                EventVoice1.Post(gameObject);
                voiceCD = Random.Range(30, 50);
            }
            else if(voiceState == 1)
            {
                
                EventVoice2.Post(gameObject);
                voiceCD = Random.Range(15, 25);
                voice1SayedNum++;
                if (voice1SayedNum > 4)
                {
                    voiceState = 2;
                }
            }
            else if (voiceState == 2)
            {
                EventVoice2.Post(gameObject);
                voiceCD = Random.Range(20, 30);
            }
        }
        //Debug.Log(isGrass + "===" + isWater);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 dircetion = new Vector3(horizontal, 0f, vertical).normalized;
        isGrounded = Physics.CheckSphere(GroundCheck.transform.position, checkerRadius, GroundMask);
        //isGrass = Physics.CheckSphere(GroundCheck.transform.position, checkerRadius, GrassMask);
        //isWater = Physics.CheckSphere(GroundCheck.transform.position, checkerRadius, WaterMask);
        //Debug.Log("isGround" + isGrounded);
        if (isGrounded && velocity.y < 0)
        {   //跳跃
            if (Input.GetKey(KeyCode.Space)&timer_jump<0)
            {
                velocity.y = 4f;
                timer_attack = 1.4f;
                timer_jump = 1.5f;
                motionController.setAnimation(13);
                if(!(dircetion.magnitude>=0.1)){
                    Invoke("SetStopAnimation", 0.85f);
                }
                Invoke("walkStep", .75f);
            }
            else
            {
                velocity.y = -5f;
            }
        }

       
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //缩减trail直至为0
        trailRenderer.time = Mathf.Lerp(trailRenderer.time, 0, 1 * Time.deltaTime);


        if (dircetion.magnitude >= 0.1&timer_attack<0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (isGrounded & velocity.y < 0) {
                        motionController.setAnimation(10);
                        speed = 4.8f;
                        
                        if (dodgeSpeedCD > 0 &dodgeCD<0)
                        {
                        //冲刺开始
                        if (!isWhooshPlayed)
                        {
                            EventWhoosh.Post(gameObject);
                            isWhooshPlayed = true;
                        }
                            trailRenderer.time = 2f;
                            speed = 18f;
                            dodgeSpeedCD -= Time.deltaTime;
                        }
                        if (dodgeSpeedCD <= 0)//冲刺结束
                        {
                            dodgeCD = 1f;
                            isWhooshPlayed = false;
                        }
                        
                        if (footStepCD < 0)
                        {
                            walkStep();
                            footStepCD = .3835f;
                        }
                    }
                }
                else
                {
                if (isGrounded & velocity.y < 0) {
                    motionController.setAnimation(6); 
                    speed = 1.9f;
                    if (footStepCD < 0)
                    {
                        walkStep();
                        footStepCD = .45f;
                    }
                }

                if (dodgeCD > 0.8)//松开shift刷新
                {
                    dodgeSpeedCD = 0.35f;
                    attack_num = 0;
                }
            }
                float targetAngle = Mathf.Atan2(dircetion.x, dircetion.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float smoothAngel = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, smoothTime);
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                transform.rotation = Quaternion.Euler(0f, smoothAngel, 0f);
                controller.Move(moveDir.normalized * speed * Time.deltaTime);

            }
            else
            {//没有按方向键
            if (isGrounded &timer_attack<0 )
                {
                    motionController.setAnimation(1);
                    timer_action = 1.5f;
                }
            }

            if (Input.GetKey(KeyCode.Mouse0) & timer_attack < 0)
            {
            
            attack();
            if (Random.Range(0, 100) < 35) { EventAttack.Post(gameObject); }
            EventSword.Post(gameObject);
            switch (attack_num%4)
            {
                case 3:
                //快速旋转
                    motionController.setAnimation(21);
                    //swordLight = 
                    Destroy(Instantiate(swordPrefabs[0], this.transform),.8f);
                    Invoke("SetStopAnimation",0.80f);
                    timer_attack = 0.80f;
                    break;
                case 0:
                //劈两下
                    motionController.setAnimation(22);
                    Destroy(Instantiate(swordPrefabs[1], this.transform), .8f);

                    Invoke("SetStopAnimation", 1.43f);
                    timer_attack = 1.13f;
                    break;
                case 1:
                //慢速旋转
                    motionController.setAnimation(23);
                    //swordLight = Instantiate(swordPrefabs[2], this.transform);
                    Destroy(Instantiate(swordPrefabs[2], this.transform), .8f);

                    Invoke("SetStopAnimation", 0.50f);
                    timer_attack = 0.50f;
                    break;
                case 2:
                //转圈
                    motionController.setAnimation(24);
                    //swordLight = Instantiate(swordPrefabs[3], this.transform);
                    Destroy(Instantiate(swordPrefabs[3], this.transform), .8f);

                    Invoke("SetStopAnimation", 0.40f);
                    timer_attack = 0.40f;
                    break;
                default:
                    motionController.setAnimation(24);
                    //swordLight = Instantiate(swordPrefabs[3], this.transform);
                    Destroy(Instantiate(swordPrefabs[3], this.transform), .8f);

                    Invoke("SetStopAnimation", 0.85f);
                    timer_attack = 0.85f;
                    break;
            }
            attack_num++;
            // attack_num++; if (attack_num > 4) { attack_num = 1; }
        }
        else{
            if(timer_attack<0){
                sword.transform.GetComponent<Collider>().enabled = false;
            }
           

        }

        if(Input.GetKey(KeyCode.Alpha1) & timer_attack < 0){
            Invoke("spell1", 1f);
            timer_attack = 1f;
            motionController.setAnimation(26);
        }
        if (Input.GetKey(KeyCode.Alpha2) & timer_attack < 0)
        {
            Invoke("spell2", 1f);
            timer_attack = 1f;
            motionController.setAnimation(27);
        }
        if (Input.GetKey(KeyCode.Alpha3) & timer_attack < 0)
        {
            Invoke("spell3", 1f);
            timer_attack = 1f;
            motionController.setAnimation(28);
        }
        




        timer_action -= Time.deltaTime;
        timer_attack -= Time.deltaTime;
        timer_jump -= Time.deltaTime;
        footStepCD -= Time.deltaTime;
        voiceCD -= Time.deltaTime;
        timer_gameStart += Time.deltaTime;
        dodgeCD -= Time.deltaTime;
        InternalLockUpdate();


    }

    private void walkStep()
    {
        if (isGrass)
        {
            
            footStepGrass.SetValue(gameObject);
            Event.Post(gameObject);
            return;
        }else if (isWater)
        {
            //
            footStepWater.SetValue(gameObject);
            Event.Post(gameObject);
            return;
        }
        //
        footStepMud.SetValue(gameObject);
        Event.Post(gameObject);
    }

    private void spell1(){

        GameObject readyToDestory=Instantiate(prefabEffect[0], transform.position + Vector3.up * 1f, transform.rotation);
        Destroy(readyToDestory, 25f);

    }
    private void spell2()
    {
        GameObject readyToDestory = Instantiate(prefabEffect[1], transform.position + Vector3.up * 1f, transform.rotation);
        Destroy(readyToDestory, 25f);

    }
    private void spell3()
    {
        GameObject readyToDestory=Instantiate(prefabEffect[2], transform.position + Vector3.up * 1f, new Quaternion(0f, .6f, -.7f, 0f));

        Destroy(readyToDestory, 25f);
    }

    private void attack(){

        //Vector3 pos = new Vector3(Player.position.x, Player.position.y+1, Player.position.z+2);
        ////Instantiate(sword, pos, this.transform.rotation);
        //// this.transform.rotation.
        ////Vector3 rot = new Vector3(this.transform.rotation.x, this.transform.rotation.y+1, -this.transform.rotation.z);
        ////Quaternion rot = new Quaternion(this.transform.rotation.x, this.transform.rotation.y , -this.transform.rotation.z,this.transform.rotation.w);
        ////sword.transform.SetPositionAndRotation(pos, rot);
        //sword.Translate(pos);
        sword.transform.GetComponent<Collider>().enabled = true;
    }

    private void SetStopAnimation(){
        motionController.setAnimation(1);
        Destroy(swordLight,.5f);
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void DisableTrailRenderer()
    {
        //trailRenderer.time = 0f;
        
    }
}// 锁定鼠标 (隐藏鼠标)
   

