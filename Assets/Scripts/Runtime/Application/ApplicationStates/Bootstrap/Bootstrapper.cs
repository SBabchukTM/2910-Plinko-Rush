using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Application.GameStateMachine
{
    public class Bootstrapper : IInitializable
    {
        private readonly StateMachine _stateMachine;
        private readonly BootstrapState _bootstrapState;
        private readonly GameState _gameState;

        public Bootstrapper(StateMachine stateMachine, BootstrapState bootstrapState, GameState gameState)
        {
            _stateMachine = stateMachine;
            _bootstrapState = bootstrapState;
            _gameState = gameState;
        }

        public void Initialize()
        {
            UnityEngine.Application.targetFrameRate = 60;
            _stateMachine.Initialize(_bootstrapState, _gameState);
            _stateMachine.GoTo<BootstrapState>().Forget();
        }
    }
}