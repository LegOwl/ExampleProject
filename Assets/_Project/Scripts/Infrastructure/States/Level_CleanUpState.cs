using StateMachine;
using Services;

namespace Infrastructure
{
    public class Level_CleanUpState : IState
    {
        private IStateChanger _stateChanger;

        public Level_CleanUpState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
        }
        public void Enter()
        {
            System.GC.Collect();
        }

        public void Exit()
        {
        }
    }
}