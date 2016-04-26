﻿using UnityEngine;

public interface IMouvement
{
    void MoveTowardsWithKeys(GameObject go, Vector2 mouvementVector, float speed);
    void MoveTowardsWithJoyStick(GameObject go, Vector2 mouvementVector, float speed);
    void MoveTo(GameObject go, Vector2 targetPos, float speed);
}

