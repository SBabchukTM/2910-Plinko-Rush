using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> levels;
    private int currentLevelIndex = 0;

    public LevelData CurrentLevel => levels[currentLevelIndex];

    public bool TryAdvanceHoleForPlayer(PlayerProgress progress)
    {
        int nextHoleIndex = progress.CurrentHoleIndex + 1;

        if (nextHoleIndex >= CurrentLevel.holes.Count)
        {
            return false;
        }
        progress.CurrentHoleIndex = nextHoleIndex;
        return true;
    }

    public void SetCurrentLevelIndex(int level)
    {
        currentLevelIndex = level;
    }

    public void Activate(int id)
    {
        DeactivateAll();
        levels[id].levelObject.SetActive(true);
    }

    public void DeactivateAll()
    {
        foreach (var level in levels)
        {
            level.levelObject.SetActive(false);
        }
    }
}