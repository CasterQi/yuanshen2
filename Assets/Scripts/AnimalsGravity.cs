using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsGravity : MonoBehaviour
{
    public CharacterController controller;
    public Transform GroundCheck;
    public LayerMask GroundMask;
    float gravity = -9.8f;
    float checkerRadius = 0.04f;
    bool isGrounded;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, checkerRadius, GroundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
