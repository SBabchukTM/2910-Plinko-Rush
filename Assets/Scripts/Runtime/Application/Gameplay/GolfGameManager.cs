using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GolfGameManager : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private BallHitController player1Ball;
    [SerializeField] private BallHitController player2Ball;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera player1Camera;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera player2Camera;

    [SerializeField] private List<PlayerProgress> players = new List<PlayerProgress>();
    [SerializeField] private GameMode _gameMode = GameMode.TwoPlayers;

    private int currentPlayerIndex = 0;
    private CancellationToken cancellationToken;

    public event Action OnNextTurn;
    public event Action OnFinish;
    public event Action<HoleResult> OnBallInHole;
    public event Action OnBallHit;

    public async Task Initial(int level, GameMode selectedMode, CancellationToken token)
    {
        cancellationToken = token;
        player1Ball.OnBallHit += () => OnBallHit?.Invoke();
        player2Ball.OnBallHit += () => OnBallHit?.Invoke();

        player1Ball.SetKinematic(true);
        player2Ball.SetKinematic(true);

        _gameMode = selectedMode;
        levelManager.SetCurrentLevelIndex(level);
        levelManager.Activate(level);
        currentPlayerIndex = 0;
        players.Clear();

        players.Add(new PlayerProgress { ball = player1Ball, CurrentHoleIndex = 0, IsActive = true });

        if (_gameMode == GameMode.TwoPlayers)
        {
            players.Add(new PlayerProgress { ball = player2Ball, CurrentHoleIndex = 0, IsActive = true });
            player2Ball.gameObject.SetActive(true);
        }
        else
        {
            player2Ball.gameObject.SetActive(false);
            player2Camera.Priority = 0;
        }

        foreach (var player in players)
        {
            try
            {
                await UpdatePlayerPositionAsync(player, token);
                await UpdateHoleVisibilityForPlayerAsync(player, token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("OperationCanceledException.");
            }
        }

        SetCurrentPlayerActive();
        SwitchCameraToCurrentPlayer();

        foreach (var hole in levelManager.CurrentLevel.holes)
        {
            hole.OnBallEnteredHole -= OnBallEnteredHole;
            hole.OnBallEnteredHole += OnBallEnteredHole;
        }
    }

    private void SetCurrentPlayerActive()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i == currentPlayerIndex)
            {
                players[i].ball.Activate();
            }
            else
            {
                players[i].ball.Deactivate();
            }
        }
    }

    private void SwitchCameraToCurrentPlayer()
    {
        if (currentPlayerIndex == 0)
        {
            player1Camera.Priority = 10;
            player2Camera.Priority = 0;
        }
        else
        {
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
        }
    }

    private async void OnBallEnteredHole(BallHitController ball, int holeIndex)
    {
        if (CheckIfAllPlayersFinished())
        {
            return;
        }

        PlayerProgress player = players.Find(p => p.ball == ball);

        int shots = ball.ShotCount;
        int par = 3;

        HoleResult result = ShotEvaluator.Evaluate(holeIndex, shots, par);
        ball.Score += result.Score;
        Debug.Log($"Player {currentPlayerIndex + 1} - Hole {holeIndex + 1}: {result.Result} ({shots} shots, Par {par}) - Score {result.Score} TotalScore: {ball.Score}");
        result.Score = ball.Score;
        OnBallInHole(result);

        bool advanced = levelManager.TryAdvanceHoleForPlayer(player);

        if (!advanced)
        {
            player.IsActive = false;
            player.ball.Deactivate();
        }
        ball.ShotCount = 0;
        await UpdatePlayerPositionAsync(player, cancellationToken);
        player.ball.Deactivate();
        await UpdateHoleVisibilityForPlayerAsync(player, cancellationToken);
    }

    public async void NextTurn()
    {
        if (CheckIfAllPlayersFinished())
        {
            OnFinish?.Invoke();
            return;
        }
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        if (!players[currentPlayerIndex].IsActive)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }

        SetCurrentPlayerActive();
        SwitchCameraToCurrentPlayer();
        await UpdateHoleVisibilityForPlayerAsync(players[currentPlayerIndex], cancellationToken);
        OnNextTurn?.Invoke();
    }

    private bool CheckIfAllPlayersFinished()
    {
        return players.TrueForAll(p => !p.IsActive);
    }

    private async Task UpdatePlayerPositionAsync(PlayerProgress player, CancellationToken token)
    {
        Transform platformGroup = levelManager.CurrentLevel.platforms[player.CurrentHoleIndex].transform;

        if (platformGroup.childCount > 0)
        {
            Transform targetChild = platformGroup.GetChild(0);

            await Task.Yield();

            token.ThrowIfCancellationRequested();

            player.ball.transform.position = targetChild.position;
        }
    }

    private async Task UpdateHoleVisibilityForPlayerAsync(PlayerProgress player, CancellationToken token)
    {
        foreach (var hole in levelManager.CurrentLevel.holes)
        {
            token.ThrowIfCancellationRequested();
            hole.SetActiveForPlayer(false);
        }

        int targetHoleIndex = player.CurrentHoleIndex;
        if (targetHoleIndex < levelManager.CurrentLevel.holes.Count)
        {
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            levelManager.CurrentLevel.holes[targetHoleIndex].SetActiveForPlayer(true);
        }
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public int GetCurrentPlayerShotCount()
    {
        return players[currentPlayerIndex].ball.ShotCount;
    }

    public int GetCurrentHoleIndex()
    {
        return players[currentPlayerIndex].CurrentHoleIndex;
    }

    public (int player1, int player2) GetScore()
    {
        int score1 = player1Ball.Score;
        int score2 = player2Ball.Score;
        return (score1, score2);
    }

    public BallHitController GetCurrentBall()
    {
        return players[currentPlayerIndex].ball;
    }
}
