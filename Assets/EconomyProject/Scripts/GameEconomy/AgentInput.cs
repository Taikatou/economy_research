using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public abstract class AgentInput<TAgent, TScreen> : MonoBehaviour where TAgent : AgentScreen<TScreen> where TScreen : Enum
    {
        public abstract EconomySystem<TAgent, TScreen> GetEconomySystem(TAgent agent);
        
        protected Dictionary<TAgent, TScreen> EconomyScreens;

        protected virtual void SetupScreens() { }

        public virtual void Start()
        {
            EconomyScreens = new Dictionary<TAgent, TScreen>();
            SetupScreens();
        }

        public TScreen GetScreen(TAgent agent, TScreen defaultTScreen)
        {
            if (!EconomyScreens.ContainsKey(agent))
            {
                EconomyScreens.Add(agent, defaultTScreen);
            }
            return EconomyScreens[agent];
        }
        
        public void ChangeScreen(TAgent agent, TScreen choice)
        {
            var system = GetEconomySystem(agent);
            var canChange = system.CanMove(agent);
            if (canChange)
            {
                EconomyScreens[agent] = choice;
            }
        }
    }
}
