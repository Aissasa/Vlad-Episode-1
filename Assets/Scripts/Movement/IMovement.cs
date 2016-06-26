using UnityEngine;

public interface IMovement
{
    void MoveTowardsWithKeys(GameObject go, Vector2 mouvementVector, float speed);
    void MoveTowardsWithJoyStick(GameObject go, Vector2 mouvementVector, float speed);
    void MoveTowards(Transform trans, Vector2 movementVector, float speed);
    void MoveTo(GameObject go, Vector2 targetPos, float speed);
}

