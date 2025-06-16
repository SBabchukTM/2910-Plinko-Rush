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

public class PausePopupStateController : StateController
{
    private readonly IUiService _uiService;
    
    public PausePopupStateController(ILogger logger, IUiService uiService) : base(logger)
    {
        _uiService = uiService;
    }

    public override async UniTask Enter(CancellationToken cancellationToken = default)
    {
        Time.timeScale = 0;
        
        PausePopup popup = await _uiService.ShowPopup(ConstPopups.PausePopup) as PausePopup;

        popup.OnContinueButtonPressed += () =>
        {
            Time.timeScale = 1;
            popup.DestroyPopup();
        };

        popup.OnHomeButtonPressed += async () =>
        {
            Time.timeScale = 1;
            popup.DestroyPopup();
            await GoTo<MenuStateController>();
        };
    }
}
