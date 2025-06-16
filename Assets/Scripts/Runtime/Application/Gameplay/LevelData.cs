using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public GameObject levelObject;
    public List<HoleController> holes;
    public List<PlatformGroup> platforms;
}