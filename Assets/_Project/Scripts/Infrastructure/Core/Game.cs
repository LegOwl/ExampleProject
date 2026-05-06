using Infrastructure;
using Saves;
using UnityEngine;
using Services;

namespace Core
{
    public class Game : MonoBehaviour
    {
        private GameStateMachine _stateMachine;
        private bool _initialized;

        private void Awake()
        {
            _initialized = false;
            
            SetFPSLimit();
            SetScreenSleep();
            SetStateMachine();
        }

        private void SetFPSLimit()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 165;
        }

        private void SetScreenSleep()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void SetStateMachine()
        {
            _stateMachine = new GameStateMachine(AllServices.Container);
            _stateMachine.Enter<GameEnterState>();
            _initialized = true;
        }

        private void OnApplicationQuit()
        {
            _stateMachine.Enter<GameAppQuitState>();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _initialized)
            {
                var playerData = AllServices.Container.Single<GlobalUserData>();
                if (playerData != null) playerData.lastExitTime.Value = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var saveService = AllServices.Container.Single<SaveLoadService>();
                if (saveService != null) saveService.SaveAll();
            }
        }
    }
}