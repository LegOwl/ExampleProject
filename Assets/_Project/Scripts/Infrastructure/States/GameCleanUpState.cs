using StateMachine;
using Services;
using UnityEngine;
using UI;
using Installers;
using Sound;


namespace Infrastructure
{
    internal class GameCleanUpState : IState
    {
        private IGameStateChanger _stateChanger;
        private AllServices _services;
        
        private UiService uiService;
        private SoundService _soundService;
        private ParticleService particleService;

        public GameCleanUpState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _services = services;
        }

        public void Enter()
        {
            var saveService = _services.Single<SaveLoadService>();
            saveService.ResetProgress();
            saveService.ClearAll();
            uiService = _services.Single<UiService>();
            _soundService = _services.Single<SoundService>();
            //_particleManager = _services.Single<ParticleManager>();
            
            _soundService?.Dispose();

            PlayerPrefs.DeleteAll();
            
            var level = GameObject.FindAnyObjectByType<LevelInstaller>();
            if (level != null)
                level.Dispose();
            
            _stateChanger.Enter<GameEnterState>();
        }
        

        public void Exit()
        {
        }
    }
}
