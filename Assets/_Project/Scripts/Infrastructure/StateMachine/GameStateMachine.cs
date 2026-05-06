using StateMachine;
using System.Collections.Generic;
using Services;
using System;

namespace Infrastructure
{
    public class GameStateMachine : StateMachineBase, IGameStateChanger
    {
        public GameStateMachine(AllServices services)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(GameEnterState)] = new GameEnterState(this, services),
                [typeof(GameCleanUpState)] = new GameCleanUpState(this, services),
                [typeof(GameAppQuitState)] = new GameAppQuitState(this, services),
                [typeof(MenuState)] = new MenuState(this, services),
                [typeof(LevelState)] = new LevelState(this, services),
            };
        }
    }
}