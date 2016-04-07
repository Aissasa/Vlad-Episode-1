using UnityEngine;

public interface IMouvement
{
    void MoveTowardsDirection(GameObject go, Vector2 mouvementVector, float speed);
    void MoveToPosition(GameObject go, Vector2 targetPos, float speed);
}

