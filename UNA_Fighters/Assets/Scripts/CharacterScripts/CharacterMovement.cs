using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update

    //If it is player 1 or player 2
    public int playerNumber;

    private Vector3 jump;
    private bool isJumping;
    private Vector3 currentPlayerPosition;
    private float moveSpeed = 5f;
    private float deaccelleration = 0.3f;
    private Vector3 accelleration;
    public Rigidbody rb;
    public void Start()
    {
        accelleration = GetComponent<Rigidbody>().velocity;
        jump = new Vector3(0, 420, 0);
        isJumping = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Floor")
        {
            isJumping = false;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.name == "Floor")
        {
            if (!Input.anyKey)
            {
                var newAccelleration = new Vector3(0, 0, 0);
                if (accelleration.x > 0)
                {
                    newAccelleration.x = accelleration.x - deaccelleration;
                    newAccelleration.y = accelleration.y;
                    GetComponent<Rigidbody>().velocity = newAccelleration;
                    if (newAccelleration.x < 0)
                    {
                        newAccelleration.x = 0;
                    }
                }
                else if(accelleration.x < 0)
                {
                    newAccelleration.x = accelleration.x + deaccelleration;
                    newAccelleration.y = accelleration.y;
                    if(newAccelleration.x > 0)
                    {
                        newAccelleration.x = 0;
                    }
                    GetComponent<Rigidbody>().velocity = newAccelleration;
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
                if (Input.GetKey(KeyCode.A))
                {
                    Move(Movement.Backward);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    Move(Movement.Forward);
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
            isJumping = true;
            GetComponent<Rigidbody>().AddForce(jump);
        }        
    }

    private void Move(Movement movement)
    {
        var newSpeed = new Vector3(0, 0, 0);
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
        }
    }


}
