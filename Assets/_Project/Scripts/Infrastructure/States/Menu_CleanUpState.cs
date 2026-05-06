using Services;
using StateMachine;

namespace Infrastructure
{
    public class Menu_CleanUpState : IState
    {
        private IStateChanger _stateChanger;

        public Menu_CleanUpState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
        }

        public void Enter()
        {
            System.GC.Collect();
        }

        public void Exit() { }
    }
}
