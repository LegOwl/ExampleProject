using Saves;
using Services;
using StateMachine;

namespace Infrastructure
{
    public class GameAppQuitState : IState
    {
        private IGameStateChanger _stateChanger;
        private GlobalUserData _globalUserData;
        private SaveLoadService _saveLoadService;

        public GameAppQuitState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _globalUserData = services.Single<GlobalUserData>();
            _saveLoadService = services.Single<SaveLoadService>();
        }
        public void Enter()
        {
            
        }

        public void Exit()
        {

        }
    }
}