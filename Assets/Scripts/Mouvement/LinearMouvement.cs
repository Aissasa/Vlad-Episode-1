using UnityEngine;
using System.Collections;
using System;

public class LinearMouvement : IMouvement
{
    protected static LinearMouvement instance;

    private LinearMouvement()
    {
    }

    public static LinearMouvement Instance()
    {
        if (instance == null)
        {
            instance = new LinearMouvement();
        }

        return instance;
    }

    public void MoveTowardsDirection(GameObject go, Vector2 movementVector, float speed = 1.5f)
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

    public void MoveToPosition(GameObject go, Vector2 targetPos, float speed = 1.5f)
    {
        Transform trans = go.GetComponent<Transform>();
        trans.position = Vector2.MoveTowards(trans.Get2DPosition(), targetPos, speed * Time.deltaTime);
    }

}
