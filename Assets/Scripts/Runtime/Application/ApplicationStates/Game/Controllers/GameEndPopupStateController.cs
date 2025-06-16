using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Application.Game;
using Application.UI;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ILogger = Core.ILogger;

public class GameEndPopupStateController : StateController
{
    private readonly IUiService _uiService;
    // private readonly GameData _gameData;

    public GameEndPopupStateController(ILogger logger, IUiService uiService) : base(logger)// GameData gameData) : base(logger)
    {
        _uiService = uiService;
        //  _gameData = gameData;
    }

    public override async UniTask Enter(CancellationToken cancellationToken = default)
    {
        Time.timeScale = 0;

        GameEndPopup popup = await _uiService.ShowPopup(ConstPopups.GameEndPopup) as GameEndPopup;

        //   popup.SetData(_gameData.CollectedCoins * _gameData.Multiplier, _gameData.Score);

        popup.OnHomePressed += async () =>
        {
            Time.timeScale = 1;
            popup.DestroyPopup();
            await GoTo<MenuStateController>();
        };

        popup.OnReplayPressed += async () =>
        {
            Time.timeScale = 1;
            popup.DestroyPopup();
            await GoTo<GameplayStateController>();
        };
    }
}
