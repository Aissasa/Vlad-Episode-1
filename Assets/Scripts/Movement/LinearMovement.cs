using UnityEngine;
using System.Collections;
using System;

public class LinearMovement : IMovement
{
    protected static LinearMovement instance;

    private LinearMovement()
    {
    }

    public static LinearMovement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LinearMovement();
            }

            return instance;
        }
    }

    // for the player
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
    
    // for the player
    public void MoveTowardsWithJoyStick(GameObject go, Vector2 movementVector, float speed)
    {
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        rb2d.MovePosition(rb2d.position + movementVector * Time.deltaTime * speed);
    }

    // for the moving objects(exp: combatText)
    public void MoveTowards(Transform trans, Vector2 movementVector, float speed)
    {
        trans.Translate( movementVector * Time.deltaTime * speed);
    }

    // for the enemies
    public void MoveTo(GameObject go, Vector2 targetPos, float speed)
    {
        Transform trans = go.GetComponent<Transform>();
        trans.position = Vector2.MoveTowards(trans.Get2DPosition(), targetPos, speed * Time.deltaTime);
    }

}
