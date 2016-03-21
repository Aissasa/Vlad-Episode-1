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

    public void Move(GameObject go, Vector2 mouvementVector, float speed = 1.5f)
    {
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();

        if (mouvementVector.x * mouvementVector.y == 0)
        {
            rb2d.MovePosition(rb2d.position + mouvementVector * Time.deltaTime * speed);
        }
        else
        {
            //this is used to insure having the same speed in all the directions
            float lateralSpeed = speed / Mathf.Sqrt(2);
            rb2d.MovePosition(rb2d.position + mouvementVector * Time.deltaTime * lateralSpeed);
        }
    }
}
