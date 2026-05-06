using Services;
using Sound;
using StateMachine;
using UniRx;

namespace Infrastructure
{
    internal class MenuState : IState
    {
        private IGameStateChanger _stateChanger;
        private AllServices _services;
        private MenuStateMachine _modeStateMachine;
        
        public MenuState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _services = services;
            _modeStateMachine = new MenuStateMachine(stateChanger, services);
        }

        public void Enter()
        {
            _modeStateMachine.Enter<Menu_ActiveState>();
        }

        public void Exit()
        {
            _modeStateMachine.Enter<Menu_CleanUpState>();
        }
    }
}
