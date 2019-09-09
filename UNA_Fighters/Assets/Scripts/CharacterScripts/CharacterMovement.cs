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

    public float animationSpeed = 0.2f;
    public float maxSpeed = 8f;
    public float jumpForce = 420;
    public float moveSpeed = 0.5f;
    public float cancelJumpForce = 15f;
    public float jumpReleaseForce = 100f;
    public float deaccelleration = 0.3f;


    private bool isJumping;
    private bool isRunning;

    private Direction currentDirection;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    public void Start()
    {
        animator.speed = animationSpeed;
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        characterAnimations = new Dictionary<string, AnimationClip>();
        GetAnimations();
        ChangeAnimation("Idle");



        accelleration = rb.velocity;
        jump = new Vector2(0, jumpForce);
        isJumping = false;
        isRunning = false;
        currentDirection = Direction.Right;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.name == "Floor")
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
        CheckDirection();

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
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    RunAnimation();
                    Move(Movement.Forward);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    Move(Movement.Down);
                }
                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Jump();
                }
                if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    DeaccellerateJump();
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    RunAnimation();
                    spriteRenderer.flipX = true;
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    RunAnimation();
                    spriteRenderer.flipX = false;
                    Move(Movement.Forward);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Move(Movement.Down);
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
        var newSpeed = new Vector2(0,0);
        newSpeed.y = rb.velocity.y;
        newSpeed.x = rb.velocity.x;
        switch (movement)
        {
            case Movement.Forward:
               if(rb.velocity.x < maxSpeed)
                {
                    newSpeed.x = rb.velocity.x + moveSpeed;
                    if(newSpeed.x > maxSpeed)
                    {
                        newSpeed.x = maxSpeed;
                    }
                    rb.velocity = newSpeed;
                }
            break;

            case Movement.Backward:
                if (rb.velocity.x > -maxSpeed)
                {
                    newSpeed.x = rb.velocity.x -  moveSpeed;
                    if (newSpeed.x < -maxSpeed)
                    {
                        newSpeed.x = -maxSpeed;
                    }
                    rb.velocity = newSpeed;
                }
            break;

            case Movement.Down:
                if (isJumping)
                {
                    newSpeed.y = -cancelJumpForce;
                    rb.AddForce(newSpeed);
                }
            break;
        }
    }


    private void GetAnimations()
    {
        Object[] animationClips = Resources.LoadAll($"Animations/Characters/{gameObject.name}/");

        for(int i = 0; i < animationClips.Length; i++)
        {
            if(animationClips[i] is AnimationClip)
            {
                characterAnimations.Add(animationClips[i].name, animationClips[i] as AnimationClip);
            }
        }
    }

    public void ChangeAnimation(string animationName)
    {
        if (characterAnimations.ContainsKey(animationName))
        {
            animatorOverrideController["Animation"] = characterAnimations[animationName];
            animator.Play("Animation", 0, 0f);
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
            onJumpReleaseVelocity.y = -jumpReleaseForce;
            rb.AddForce(onJumpReleaseVelocity);
        }
    }

    public void CrouchAnimation()
    {

    }

    public void CheckDirection()
    {
        var flip = new Vector3(0, 180, 0);
        transform.Rotate(flip);
    }

}
