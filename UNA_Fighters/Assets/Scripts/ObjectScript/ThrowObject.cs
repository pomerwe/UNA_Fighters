using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{

    public GameObject whoThrowed;

    public bool rotate = false;

    private void Start()
    {
        Physics2D.IgnoreCollision(whoThrowed.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void Update()
    {
        if (rotate)
        {
            gameObject.transform.Rotate(0, 0, 3);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.rigidbody != null)
        {
            if (col.rigidbody.gameObject.transform.parent.gameObject.name == "Characters")
            {
                if (col.rigidbody.gameObject != whoThrowed)
                {
                    col.rigidbody.gameObject.GetComponent<CharacterStats>().character.HP -= 15f;
                }
            }

            if (col.rigidbody.gameObject != whoThrowed)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

}
