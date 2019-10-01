using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{

    public GameObject whoThrowed;
    private void Start()
    {
        //var stopwatch = new Stopwatch();
        //stopwatch.Start();
        //while (stopwatch.Elapsed.TotalSeconds <= 10)
        //{

        //}
        //stopwatch.Stop();
        //Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D col)
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

}
