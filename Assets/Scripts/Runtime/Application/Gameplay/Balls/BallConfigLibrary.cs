using Application.Game;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallConfigLibrary", menuName = "Config/BallConfigLibrary")]
public class BallConfigLibrary : ScriptableObject
{
    [SerializeField] private List<BallTypeConfigPair> configs;

    public BallConfig GetConfig(BallType type)
    {
        foreach (var pair in configs)
        {
            if (pair.Type == type)
                return pair.Config;
        }
        return null;
    }

    [System.Serializable]
    public struct BallTypeConfigPair
    {
        public BallType Type;
        public BallConfig Config;
    }
}