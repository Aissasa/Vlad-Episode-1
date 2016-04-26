using UnityEngine;
using System.Collections;
using System;

public class LinearMouvement : IMouvement
{
    protected static LinearMouvement instance;

    private LinearMouvement()
    {
    }

    public static LinearMouvement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LinearMouvement();
            }

            return instance;
        }
    }

    public void MoveTowardsWithKeys(GameObject go, Vector2 movementVector, float speed)
    {
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        if (movementVector.x * movementVector.y == 0)
        {
            rb2d.MovePosition(rb2d.position + movementVector * Time.deltaTime * speed);
        }
        else
        {
            //this is used to insure having the same speed in all the directions
            float diagonalSpeed = speed / Mathf.Sqrt(2);
            rb2d.MovePosition(rb2d.position + movementVector * Time.deltaTime * diagonalSpeed);
        }
    }

    public void MoveTowardsWithJoyStick(GameObject go, Vector2 movementVector, float speed)
    {
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        rb2d.MovePosition(rb2d.position + movementVector * Time.deltaTime * speed);
    }

    public void MoveTowards(Transform trans, Vector2 movementVector, float speed)
    {
        trans.Translate( movementVector * Time.deltaTime * speed);
    }

    public void MoveTo(GameObject go, Vector2 targetPos, float speed)
    {
        Transform trans = go.GetComponent<Transform>();
        trans.position = Vector2.MoveTowards(trans.Get2DPosition(), targetPos, speed * Time.deltaTime);
    }

}
