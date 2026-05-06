using StateMachine;
using System.Collections.Generic;
using Services;
using System;

namespace Infrastructure
{
    public class LevelStateMachine : StateMachineBase, IStateChanger
    {
        public LevelStateMachine(IGameStateChanger gameStateChanger, AllServices services)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(Level_ActiveState)] = new Level_ActiveState(this, gameStateChanger, services),
                [typeof(Level_CleanUpState)] = new Level_CleanUpState(this, gameStateChanger, services),
            };
        }
    }
}