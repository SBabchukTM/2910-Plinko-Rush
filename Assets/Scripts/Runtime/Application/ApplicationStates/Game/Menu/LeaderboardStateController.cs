using System.Threading;
using Application.UI;
using Core;
using Core.StateMachine;
using Cysharp.Threading.Tasks;

namespace Application.Game
{
    public class LeaderboardStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly RecordsFactory _recordsFactory;

        private LeaderboardScreen _screen;

        public LeaderboardStateController(ILogger logger, IUiService uiService, RecordsFactory recordsFactory) : base(logger)
        {
            _uiService = uiService;
            _recordsFactory = recordsFactory;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.LeaderboardScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<LeaderboardScreen>(ConstScreens.LeaderboardScreen);
            _screen.Initialize(_recordsFactory.CreateRecordDisplayList());
            _screen.ShowAsync().Forget();
        }
    }
}