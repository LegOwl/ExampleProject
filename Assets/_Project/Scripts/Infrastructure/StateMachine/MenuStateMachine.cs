using System;
using System.Collections.Generic;
using Services;
using StateMachine;

namespace Infrastructure
{
    public class MenuStateMachine : StateMachineBase, IStateChanger
    {
        public MenuStateMachine(IGameStateChanger gameStateChanger, AllServices services)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(Menu_ActiveState)] = new Menu_ActiveState(this, gameStateChanger, services),
                [typeof(Menu_CleanUpState)] = new Menu_CleanUpState(this, gameStateChanger, services),
            };
        }
    }
}
