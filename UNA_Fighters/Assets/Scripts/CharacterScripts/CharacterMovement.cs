
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Dictionary<string, AnimationClip> characterAnimations;
    
    //If it is player 1 or player 2
    public int playerNumber;

    private Vector3 currentPlayerPosition;
    private Vector2 jump;

    public float attackCount;

    private float currentAttack = 1;

    public float animationSpeed = 0.2f;
    public float attackSpeed = 2f;
    public float maxSpeed = 8f;
    public float jumpForce = 420;
    public float moveSpeed = 0.5f;
    public float cancelJumpForce = 15f;
    public float jumpReleaseForce = 100f;
    public float deaccelleration = 0.6f;
    public float maxfallDownSpeed = -10f;
    public float weightValue = 5f;

    public int jumpCount;

    public bool isAttacking = false;
    public bool isIdle;
    public bool isJumping;
    public bool isRunning;
    public bool isFlipped;
    public bool isCancelingJump;
    public bool isCrouching;
    public bool crouchHolded;
    public bool isGuarding;
    public bool takingHit = false;

    public bool canThrow = false;

    private Direction lastDirection;
    private Direction currentDirection;

    public BoxCollider2D bc;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    public void Start()
    {

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        NormalAnimationsConfig();
        
        GetAnimations();
        ChangeAnimation("Idle");


        crouchHolded = false;
        jumpCount = 0;

        jump = new Vector2(0, jumpForce);
        isJumping = false;
        isRunning = false;
        isFlipped = false;
        isCancelingJump = false;
        isCrouching = false;
        isIdle = true;
        isGuarding = false;
        currentDirection = Direction.Right;
        lastDirection = currentDirection;
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.transform.parent.gameObject.name == "Characters")
        {
            if (col.otherCollider is CapsuleCollider2D)
            {
                OnAttackWork(col.collider);
            }
        }
        //if (col.gameObject.name == "InvisibleWall")
        //{
        //    Destroy(gameObject);
        //    return;
        //}
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
            else if(isGuarding)
            {
                ChangeAnimation("Guard");
            }
            else if(isRunning)
            {
                ChangeAnimation("Run");
            }

            jumpCount = 0;
            isJumping = false;
            isCancelingJump = false;
        }


    }

    void OnCollisionStay2D(Collision2D col)
    {
        CheckIdleness();

        if (col.gameObject.name == "Character")
        {
            CheckCharacterPushing(col);
        }
            

        if (col.gameObject.name == "Floor")
        {
            if (!CheckPlayerButton() || isGuarding)
            {
                Deaccellerate();
            }

            if (rb.velocity.x == 0 && isIdle)
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
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    Jump();
                }
                if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick1Button5))
                {
                    Guard();
                }
                if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.Joystick1Button5))
                {
                    StopGuard();
                }
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Joystick1Button0))
                {
                    DeaccellerateJump();
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Joystick1Button14) || (Input.GetAxis("p1x") < 0))
                {
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Joystick1Button15) || (Input.GetAxis("p1x") > 0))
                {
                    Move(Movement.Forward);
                }
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Joystick1Button13) || (Input.GetAxis("p1y") < -0.4f))
                {
                    Move(Movement.Down);
                }
                if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.Joystick1Button13) || (Input.GetAxis("p1y") == 0))
                {
                    CancelCrouch();
                }
                if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button2))
                {
                    Attack();
                }
                if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Joystick1Button3))
                {
                    Throw();
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
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        Attack();
                    }

                    if (Input.GetKeyDown(KeyCode.L))
                    {

                        Throw();
                    }
                }
                break;
        }
    }

    private void Throw()
    {
        if (CanThrow())
        {
            if (canThrow)
            {
                if(GetComponent<CharacterStats>().character.Stamina == 100)
                {
                    GetComponent<CharacterStats>().character.Stamina = 0;
                    Stop();
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        AttackAnimation($"Special01");
                        var throwComponent = GetComponent<CharacterThrow>();
                        throwComponent.ThrowObject(isFlipped ? Direction.Left : Direction.Right);
                    }
                }
            }
        }
    }

    private void Attack()
    {
        if (!isJumping)
        {
            if (CanAttack())
            {
                Stop();

                var n = new System.Random();
                currentAttack = n.Next(1,(int)attackCount + 1);

                if (!isAttacking)
                {
                    isAttacking = true;
                    AttackAnimation($"Attack0{currentAttack}");
                    currentAttack++;
                }
            }
        }
        else
        {
            AttackJump();
        }
        
    }

    private void AttackJump()
    {
        if (currentAttack > attackCount)
        {
            currentAttack = 1;
        }

        if (!isAttacking)
        {
            isAttacking = true;
            AttackAnimation($"Attack0{currentAttack}");
            currentAttack++;
        }
    }

    private void Jump()
    {
        if (CanJump())
        {
            if (!isJumping || jumpCount < 2)
            {
                Vector2 cancelGravityOnJumpForce = new Vector2(rb.velocity.x, rb.velocity.y);
                cancelGravityOnJumpForce.y = 0;
                rb.velocity = cancelGravityOnJumpForce;
                ChangeAnimation("Jump");
                isJumping = true;
                jumpCount++;
                rb.AddForce(jump);
            }
        }
    }

    

    private void Move(Movement movement)
    {
        if (!isGuarding && !isAttacking)
        {
            var newSpeed = new Vector2(0, 0);
            newSpeed.y = rb.velocity.y;
            newSpeed.x = rb.velocity.x;
            switch (movement)
            {
                case Movement.Forward:
                    currentDirection = Direction.Right;
                    if (!crouchHolded)
                    {
                        if (!isJumping)
                        {
                            RunAnimation();
                        }
                        if(rb.velocity.x < 0)
                        { 
                            Stop();
                        }
                        if (rb.velocity.x < maxSpeed)
                        {
                            newSpeed.x = !isJumping ? rb.velocity.x + moveSpeed : rb.velocity.x + moveSpeed/4;
                            if (newSpeed.x > maxSpeed)
                            {
                                newSpeed.x = maxSpeed;
                            }
                            rb.velocity = newSpeed;
                        }
                    }
                    break;

                case Movement.Backward:
                    currentDirection = Direction.Left;
                    if (!crouchHolded)
                    {
                        if (!isJumping)
                        {
                            RunAnimation();
                        }
                        if (rb.velocity.x > 0)
                        {
                            Stop();
                        }
                        if (rb.velocity.x > -maxSpeed)
                        {
                            newSpeed.x = !isJumping ? rb.velocity.x - moveSpeed : rb.velocity.x - moveSpeed / 4;
                            if (newSpeed.x < -maxSpeed)
                            {
                                newSpeed.x = -maxSpeed;
                            }
                            rb.velocity = newSpeed;
                        }
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
                    else
                    {
                        Crouch();
                    }
                    break;
            }
        }        
    }


    private void GetAnimations()
    {
        characterAnimations = new Dictionary<string, AnimationClip>();
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
        if(takingHit || isGuarding && (animatorOverrideController["Animation"] == characterAnimations["Guard"])){
            return;
        }
        if (characterAnimations.ContainsKey(animationName))
        {
            NormalAnimationsConfig();
            if (animatorOverrideController["Animation"] != characterAnimations[animationName] || animationName.Equals("Jump"))
            {
                animatorOverrideController["Animation"] = characterAnimations[animationName];
                animator.Play("Animation", 0, 0f);
                isAttacking = false;
            }
        }
    }

    public void AttackAnimation(string animationName)
    {
        if (characterAnimations.ContainsKey(animationName))
        {
            if (animatorOverrideController["Animation"] != characterAnimations[animationName])
            {
                AttackAnimationsConfig();
                animatorOverrideController["Animation"] = characterAnimations[animationName];
            }
        }
        animator.Play("Attack", 0, 0f);
    }

    public void RunAnimation()
    {
        ChangeAnimation("Run");
        isRunning = true;
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

    public void Crouch()
    {
        if (CanCrouch())
        {
            ChangeAnimation("Crouch");
            Stop();
            isCrouching = true;
            crouchHolded = true;
        }
    }

    

    public void CancelCrouch()
    {
        crouchHolded = false;
        isCrouching = false;
    }

    public void CheckDirection()
    {
        var flip = new Vector3(0, 180, 0);
        var barStats = GetComponent<CharacterStats>().StatsBar;
        var barPos = barStats.transform.localPosition;
        if (currentDirection == Direction.Right)
        {
            if (isFlipped)
            {
                barStats.transform.Rotate(flip);
                barPos.x = -barPos.x;
                barStats.transform.localPosition = barPos;
                transform.Rotate(flip);
                isFlipped = false;
            }
        }
        else if (currentDirection == Direction.Left)
        {
            if (!isFlipped)
            {
                barStats.transform.Rotate(flip);
                barPos.x = -barPos.x;
                barStats.transform.localPosition = barPos;
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

    private bool CanJump()
    {
        return !isGuarding && !isCrouching && !isAttacking;
    }

    private bool CanAttack()
    {
        return !isJumping && !isGuarding;
    }

    private bool CanThrow()
    {
        return !isGuarding;
    }

    private bool CanCrouch()
    {
        return !isAttacking && !isJumping;
    }


    public void CheckIdleness()
    {
        if(isJumping | isRunning | isAttacking | isCrouching | isGuarding)
        {
            isIdle = false;
        }
        else
        {
            isIdle = true;
        }
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0, 0);
    }

    public void Guard()
    {
        if (!isGuarding && !isJumping)
        {
            Stop();
            isGuarding = true;
            ChangeAnimation("Guard");
        }
    }

    public void StopGuard()
    {
        isGuarding = false;
    }

    private void NormalAnimationsConfig()
    {
        animator.speed = animationSpeed;
    }

    private void AttackAnimationsConfig()
    {
        animator.speed = attackSpeed;
    }

    public void OnAttackWork(Collider2D target)
    {
        var distance = PositionRelative(gameObject.transform.position, target.gameObject.transform.position);
        bool willDefend = (distance < 0 && !target.gameObject.GetComponent<CharacterMovement>().isFlipped) ||
            (distance > 0 && target.gameObject.GetComponent<CharacterMovement>().isFlipped)
            ? true : false;
        if (!(target.gameObject.GetComponent<CharacterMovement>().isGuarding && willDefend))
        {
            var n = new System.Random();
            target.gameObject.GetComponent<CharacterStats>().character.HP -= 5;
            target.gameObject.GetComponent<CharacterMovement>().AttackAnimation($"Hit0{n.Next(0, 2)}");
            target.gameObject.GetComponent<CharacterMovement>().takingHit = true;
        }
        
    }

    public float PositionRelative(Vector2 pos, Vector2 pos2)
    {
        return (pos2.x - pos.x);
    }

    public void CheckCharacterPushing(Collision2D col)
    {
    }

    public bool CheckPlayerButton()
    {
        switch (playerNumber)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.Joystick1Button5))
                {
                    return true;
                }
                else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.Joystick1Button5))
                {
                    return true;
                }
                else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Joystick1Button0))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Joystick1Button14))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Joystick1Button15))
                {
                    return true;
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Joystick1Button13))
                {
                    return true;
                }
                else if(Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.Joystick1Button13))
                {
                    return true;
                }
                else if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button2))
                {
                    return true;
                }
                else if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Joystick1Button3))
                {
                    return true;
                }
                else if ((Input.GetAxis("p1x") != 0 || (Input.GetAxis("p1y") != 0)))
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



