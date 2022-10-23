using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private float moveSpeed;       //This is just to have more control over the speed.
    [SerializeField] private float runSpeed;

    private Vector3 moveDirection;
    private Vector3 velocity;

    //Check if we are on the ground. 
    [SerializeField] private bool isGrounded; //Check if player is on ground or not.
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight;

    //References
    private CharacterController controller;
    private Animator anim;
    
    private void Start(){
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

    }

    private void Update(){
        Move();
    }

    private void Move(){
        
        
        //Draw a sphere at a position with a radius and filters out the groundmask. Its going to draw
        //a small sphere at the layer of our feet and is going to check if we are standing on the ground
        //or not. Its gonna return true if we stand on the ground. 
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        //Debug.Log(isGrounded);
        
        //Checks if we are grounded. Grounded? Apply gravity aka ground us. 
        if(isGrounded && velocity.y < 0){
            anim.SetBool("Jump", false);
            velocity.y = -2f;
        }

        float moveZ = Input.GetAxis("Vertical");    //W & S keys
        moveDirection = new Vector3(0, 0, moveZ);
        moveDirection = transform.TransformDirection(moveDirection); //So we always use the players forward when we use the mouse. 

        
        if(isGrounded){
            //If you press key W or S
            if(moveDirection != Vector3.zero && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))){
                if (velocity.y < 0){
                    Run();
                }
            }

            else if(moveDirection == Vector3.zero){
                if (velocity.y < 0){
                    Idle();
                }
            }
            
            if(Input.GetKeyDown(KeyCode.Space)){
                if (velocity.y < 0){
                    Jump();
                }
            }

            moveDirection *= moveSpeed; 
        }
        
        controller.Move(moveDirection * Time.deltaTime);

        //Give character gravity
        velocity.y += gravity * Time.deltaTime;         //calculate gravity
        controller.Move(velocity * Time.deltaTime);     //apply gravity to character
    }

    private void Idle(){
        anim.SetFloat("Blend", 0, 0.1f, Time.deltaTime);
    }

    private void Run(){
        moveSpeed = runSpeed;
        anim.SetFloat("Blend", 0.5f, 0.1f, Time.deltaTime);
    }

    private void Jump(){
        anim.SetBool("Jump", true);
        //moveSpeed = runSpeed;
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity); //The amount we want to jump
    }

}
