using UnityEngine;

public interface IMouvement
{
    void MoveTowards(GameObject go, Vector2 mouvementVector, float speed);
    void MoveTo(GameObject go, Vector2 targetPos, float speed);
}

