using System;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    [SerializeField] private int _holeIndex;
    [SerializeField] private Collider2D _holeCollider;
    [SerializeField] private LevelManager _levelManager;

    public Action<BallHitController, int> OnBallEnteredHole;

    private void OnTriggerEnter2D(Collider2D other)
    {
        BallHitController ball = other.GetComponent<BallHitController>();
        if (ball != null)
        {
            _holeCollider.enabled = false;
            OnBallEnteredHole?.Invoke(ball, _holeIndex);
        }
    }

    public void SetActiveForPlayer(bool active)
    {
        _holeCollider.enabled = active;
        GetComponent<SpriteRenderer>().color = active ? Color.white : new Color(1, 1, 1, 0.3f);
    }
}