using Application.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "BallConfig", menuName = "Config/BallConfig")]
public class BallConfig : ScriptableObject
{
    [SerializeField] private BallType _ballType;
    [SerializeField] private float _mass;
    [SerializeField] private bool _isSlime;
    [SerializeField] private Sprite _itemSprite;

    public Sprite ItemSprite => _itemSprite;
    public float Mass => _mass;
    public bool IsSlime => _isSlime;
    public BallType BallType => _ballType;
}