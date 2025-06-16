using UnityEngine;

public class TrapZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BallHitController ball = other.GetComponent<BallHitController>();
        if (ball != null)
        {
            ball.ReturnToLastSafePosition();
        }
    }
}
