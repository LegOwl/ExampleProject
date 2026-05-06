using Services;
using IState = StateMachine.IState;

namespace Infrastructure
{
    internal class LevelState : IState
    {
        private IGameStateChanger _stateChanger;
        private LevelStateMachine _modeStateMachine;
        private AllServices _services;

        public LevelState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _modeStateMachine = new LevelStateMachine(stateChanger, services);
            _services = services;
        }

        public void Enter()
        {
            _modeStateMachine.Enter<Level_ActiveState>();
        }

        public void Exit()
        {
            _modeStateMachine.Enter<Level_CleanUpState>();
        }
    }
}