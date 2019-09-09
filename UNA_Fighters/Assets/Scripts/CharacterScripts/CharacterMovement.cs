using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Dictionary<string, AnimationClip> characterAnimations;

    //If it is player 1 or player 2
    public int playerNumber;

    private Vector2 currentPlayerPosition;
    private Vector2 jump;
    private Vector3 accelleration;

    public float jumpForce = 420;
    public float moveSpeed = 5f;
    public float cancelJumpSpeed = 15f;
    public float jumpReleaseSpeed = 100f;
    public float deaccelleration = 0.3f;


    private bool isJumping;
    private bool isRunning;


    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    public void Start()
    {
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        characterAnimations = new Dictionary<string, AnimationClip>();
        GetAnimations();
        animatorOverrideController["Animation"] = characterAnimations["Idle"];



        accelleration = rb.velocity;
        jump = new Vector2(0, jumpForce);
        isJumping = false;
        isRunning = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.name == "Floor")
        {
            Vector2 onFloorCollideVelocity = rb.velocity;
            onFloorCollideVelocity.y = 0;
            rb.velocity = onFloorCollideVelocity;
            if (!isRunning)
            {
                ChangeAnimation("Idle");
            }
            else
            {
                ChangeAnimation("Run");
            }
            isJumping = false;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.name == "Floor")
        {
            if (!Input.anyKey)
            {
                var newAccelleration = new Vector2(0, 0);
                if (accelleration.x > 0)
                {
                    newAccelleration.x = accelleration.x - deaccelleration;
                    newAccelleration.y = accelleration.y;
                    if (newAccelleration.x < 0)
                    {
                        newAccelleration.x = 0;
                        ChangeAnimation("Idle");
                        isRunning = false;
                    }
                    rb.velocity = newAccelleration;
                }
                else if(accelleration.x < 0)
                {
                    newAccelleration.x = accelleration.x + deaccelleration;
                    newAccelleration.y = accelleration.y;
                    if(newAccelleration.x > 0)
                    {
                        newAccelleration.x = 0;
                        ChangeAnimation("Idle");
                        isRunning = false;
                    }
                    rb.velocity = newAccelleration;
                }
               
            }           
        }
    }

    // Update is called once per frame
    private void Update()
    {
        accelleration = rb.velocity;
        switch (playerNumber)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Jump();
                }
                if(Input.GetKeyUp(KeyCode.W))
                {
                    DeaccellerateJump();
                }
                if (Input.GetKey(KeyCode.A))
                {
                    RunAnimation();
                    spriteRenderer.flipX = true;
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    RunAnimation();
                    spriteRenderer.flipX = false;
                    Move(Movement.Forward);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    Move(Movement.Down);
                }
                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.Keypad8))
                {
                    Jump();
                }
            break;
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            ChangeAnimation("Jump");
            isJumping = true;
            rb.AddForce(jump);
        }        
    }

    private void Move(Movement movement)
    {
        var newSpeed = new Vector2(0, 0);
        var currentSpeed = rb.velocity;
        switch (movement)
        {
            case Movement.Forward:
               if(currentSpeed.x < 6)
                {
                    newSpeed.x = moveSpeed;
                    rb.AddForce(newSpeed);
                }
            break;

            case Movement.Backward:
                if (currentSpeed.x > -6)
                {
                    newSpeed.x = -moveSpeed;
                    rb.AddForce(newSpeed);
                }
            break;

            case Movement.Down:
                if (isJumping)
                {
                    newSpeed.y = -cancelJumpSpeed;
                    rb.AddForce(newSpeed);
                }
            break;
        }
    }


    private void GetAnimations()
    {
        Object[] animationClips = Resources.LoadAll($"Animations/{gameObject.name}/");

        for(int i = 0; i < animationClips.Length; i++)
        {
            if(animationClips[i] is AnimationClip)
            {
                characterAnimations.Add(animationClips[i].name, animationClips[i] as AnimationClip);
            }
        }

        for (int i = 0; i < animatorOverrideController.animationClips.Length; i++)
        {
            Debug.Log(animatorOverrideController.animationClips[i].name);
        }  
    }

    public void ChangeAnimation(string animationName)
    {
        if (characterAnimations.ContainsKey(animationName))
        {
            animatorOverrideController["Animation"] = characterAnimations[animationName];
            animator.Play("CurrentAnimation", 0, 0f);
        }
    }

    public void RunAnimation()
    {
        if (!isRunning)
        {
            ChangeAnimation("Run");
        }
        isRunning = true;
    }

    public void DeaccellerateJump()
    {
        if(rb.velocity.y > 4)
        {
            Vector2 onJumpReleaseVelocity = new Vector2(0, 0);
            onJumpReleaseVelocity.y = -jumpReleaseSpeed;
            rb.AddForce(onJumpReleaseVelocity);
        }
    }

    public void CrouchAnimation()
    {

    }

}
