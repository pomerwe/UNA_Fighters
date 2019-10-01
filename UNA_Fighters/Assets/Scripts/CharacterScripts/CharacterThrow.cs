using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterThrow : MonoBehaviour
{

    public GameObject throwPrefab;

    public void ThrowObject(Direction direction)
    {
        var throwedObj = Instantiate(throwPrefab);
        throwedObj.GetComponent<ThrowObject>().whoThrowed = gameObject;
        throwedObj.transform.position = gameObject.transform.position;
        throwedObj.transform.SetParent(GameObject.Find("Scene").transform);
        var rotate = new Vector3(0, 0, 0);
        var speed = new Vector2(0, 0);
        var rigidBody = throwedObj.GetComponent<Rigidbody2D>();
        switch (direction)
        {
            case Direction.Up:
                
                rotate.z = -90;
                throwedObj.transform.Rotate(rotate);

                speed.y = +15f;
                rigidBody.velocity = speed;
                break;

            case Direction.Down:
                rotate.z = 90;
                throwedObj.transform.Rotate(rotate);

                speed.y = -15f;
                rigidBody.velocity = speed;
                break;

            case Direction.Left:

                rotate.z = 180;
                throwedObj.transform.Rotate(rotate);

                speed.x = -15f;
                rigidBody.velocity = speed;
                break;

            case Direction.Right:
                speed.x = 15f;
                rigidBody.velocity = speed;
            break;
        }
    }
}
