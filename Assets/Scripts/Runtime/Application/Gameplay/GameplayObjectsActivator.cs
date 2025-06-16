using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameplayObjectsActivator : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GameObject _ballsObject;
    [SerializeField] private GolfGameManager _golfGameManager;

    public void Activate()
    {
        _levelManager.gameObject.SetActive(true);
        _ballsObject.SetActive(true);
        _golfGameManager.gameObject.SetActive(true);
    }

    public async Task Deactivate()
    {
        _levelManager.gameObject.SetActive(false);
        _ballsObject.SetActive(false);
        _golfGameManager.gameObject.SetActive(false);
        await Task.CompletedTask;
    }
}