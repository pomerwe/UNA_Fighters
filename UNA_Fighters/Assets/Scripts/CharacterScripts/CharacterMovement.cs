using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Dictionary<string, AnimationClip> characterAnimations;

    //If it is player 1 or player 2
    public int playerNumber;

    private Vector3 currentPlayerPosition;
    private Vector2 jump;

    public float animationSpeed = 0.2f;
    public float maxSpeed = 8f;
    public float jumpForce = 420;
    public float moveSpeed = 0.5f;
    public float cancelJumpForce = 15f;
    public float jumpReleaseForce = 100f;
    public float deaccelleration = 0.6f;
    public float maxfallDownSpeed = -10f;
    public float weightValue = 5f;

    public bool isAttacking = false;
    private bool isIdle;
    private bool isJumping;
    private bool isRunning;
    private bool isFlipped;
    private bool isCancelingJump;

    private Direction lastDirection;
    private Direction currentDirection;

    public BoxCollider2D bc;
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



       
        jump = new Vector2(0, jumpForce);
        isJumping = false;
        isRunning = false;
        isFlipped = false;
        isCancelingJump = false;
        isIdle = true;
        currentDirection = Direction.Right;
        lastDirection = currentDirection;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.name == "Floor")
        {
            Vector2 onFloorCollideVelocity = rb.velocity;
            onFloorCollideVelocity.y = 0;
            if (isCancelingJump && !isJumping)
            {
                if (onFloorCollideVelocity.x < 0)
                {
                    onFloorCollideVelocity.x = -maxSpeed / 2;
                }
                else
                {
                    onFloorCollideVelocity.x = maxSpeed / 2;
                }

            }
            rb.velocity = onFloorCollideVelocity;

            if (isIdle)
            {
                ChangeAnimation("Idle");
            }
            else
            {
                ChangeAnimation("Run");
            }
            isJumping = false;
            isCancelingJump = false;
        }


    }

    void OnCollisionStay2D(Collision2D col)
    {

        if (col.gameObject.transform.parent.gameObject.name == "Characters")
        {

        }

        if (col.gameObject.name == "Floor")
        {
            if (!CheckPlayerButton())
            {
                Deaccellerate();
            }

            if (rb.velocity.x == 0 && !isJumping && !isRunning && !isAttacking)
            {
                ChangeAnimation("Idle");
            }
        }

       
    }

    // Update is called once per frame
    private void Update()
    {

        CheckIdleness();

        CheckSpeed();

        currentPlayerPosition = transform.position;

        if (currentDirection != lastDirection)
        {
            CheckDirection();
            lastDirection = currentDirection;
        }

        switch (playerNumber)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Jump();
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    DeaccellerateJump();
                }
                if (Input.GetKey(KeyCode.A))
                {
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    Move(Movement.Forward);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    Move(Movement.Down);
                }
                break;

            case 2:
                if (!isAttacking)
                {
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

                        Move(Movement.Backward);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        Move(Movement.Forward);
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        Move(Movement.Down);
                    }
                }
                
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        AttackAnimation("Attack01");
                        
                    }
                }
                break;
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            Vector2 cancelGravityOnJumpForce = new Vector2(rb.velocity.x, rb.velocity.y);
            cancelGravityOnJumpForce.y = 0;
            rb.velocity = cancelGravityOnJumpForce;
            ChangeAnimation("Jump");
            isJumping = true;
            rb.AddForce(jump);
        }
    }

    private void Move(Movement movement)
    {
        var newSpeed = new Vector2(0, 0);
        newSpeed.y = rb.velocity.y;
        newSpeed.x = rb.velocity.x;
        switch (movement)
        {
            case Movement.Forward:

                if (!isJumping)
                {
                    RunAnimation();
                }
                if (rb.velocity.x < maxSpeed)
                {
                    currentDirection = Direction.Right;
                    newSpeed.x = rb.velocity.x + moveSpeed;
                    if (newSpeed.x > maxSpeed)
                    {
                        newSpeed.x = maxSpeed;
                    }
                    rb.velocity = newSpeed;
                }
                break;

            case Movement.Backward:

                if (!isJumping)
                {
                    RunAnimation();
                }
                if (rb.velocity.x > -maxSpeed)
                {
                    currentDirection = Direction.Left;
                    newSpeed.x = rb.velocity.x - moveSpeed;
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
                    if (rb.velocity.y > maxfallDownSpeed)
                    {
                        CancelJump();
                    }
                    
                }
                break;
        }
    }


    private void GetAnimations()
    {
        Object[] animationClips = Resources.LoadAll($"Animations/Characters/{gameObject.name}/");

        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i] is AnimationClip)
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
            isAttacking = false;
        }
    }

    public void AttackAnimation(string animationName)
    {
        if (characterAnimations.ContainsKey(animationName))
        {
            animatorOverrideController["Attack"] = characterAnimations[animationName];
            animator.Play("Attack", 0, 0f);
        }
    }

    public void RunAnimation()
    {
        if (!isRunning)
        {
            ChangeAnimation("Run");
            isRunning = true;
        }
    }

    public void DeaccellerateJump()
    {
        if (rb.velocity.y > 4)
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
        if (currentDirection == Direction.Right)
        {
            if (isFlipped)
            {
                transform.Rotate(flip);
                isFlipped = false;
            }
        }
        else if (currentDirection == Direction.Left)
        {
            if (!isFlipped)
            {
                transform.Rotate(flip);
                isFlipped = true;
            }
        }

    }


    public void CheckSpeed()
    {
        Vector2 ControlledSpeed = rb.velocity;
        if (rb.velocity.x > maxSpeed)
        {
            ControlledSpeed.x = maxSpeed;
            rb.velocity = ControlledSpeed;
        }
        if(rb.velocity.x < -maxSpeed)
        {
            ControlledSpeed.x = -maxSpeed;
            rb.velocity = ControlledSpeed;
        }
    }

    public void Deaccellerate()
    {
        var newAccelleration = new Vector2(0, 0);
        if (rb.velocity.x > 0)
        {
            newAccelleration.x = rb.velocity.x - deaccelleration;
            newAccelleration.y = rb.velocity.y;
            if (newAccelleration.x < 0)
            {
                newAccelleration.x = 0;
                ChangeAnimation("Idle");
                isRunning = false;
            }
            rb.velocity = newAccelleration;
        }
        else if (rb.velocity.x < 0)
        { 
            newAccelleration.x = rb.velocity.x + deaccelleration;
            newAccelleration.y = rb.velocity.y;
            if (newAccelleration.x > 0)
            {
                newAccelleration.x = 0;
                ChangeAnimation("Idle");
                isRunning = false;
            }
            rb.velocity = newAccelleration;
        }
        else
        {
            if (isRunning)
            {
                isRunning = false;
            }
        }
    }

    public void CancelJump()
    {
        Vector2 newSpeed = rb.velocity;
        isCancelingJump = true;
        newSpeed.y = -cancelJumpForce;
        rb.AddForce(newSpeed);
        
    }

    public void CheckIdleness()
    {
        if(isJumping | isRunning | isAttacking)
        {
            isIdle = false;
        }
        else
        {
            isIdle = true;
        }
    }


    public bool CheckPlayerButton()
    {
        switch (playerNumber)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    return true;
                }
                else if (Input.GetKeyUp(KeyCode.W))
                {
                    return true;
                }
                else if(Input.GetKey(KeyCode.A))
                {
                    return true;
                }
                else if(Input.GetKey(KeyCode.D))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 2:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    return true;
                }
                else if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return true;
    }

}



