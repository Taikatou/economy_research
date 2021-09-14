using System;
using System.Collections.Generic;
using EconomyProject.Scripts.GameEconomy.Systems;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public abstract class AgentInput<TAgent, TScreen, TInput> : MonoBehaviour where TAgent : AgentScreen<TScreen> where TScreen : Enum where TInput : Enum
    {
        public abstract EconomySystem<TAgent, TScreen, TInput> GetEconomySystem(TAgent agent);
        
        protected Dictionary<TAgent, TScreen> EconomyScreens;
        private Dictionary<TScreen, int> _economyCount;

        protected virtual void SetupScreens() { }

        public virtual void Awake()
        {
            EconomyScreens = new Dictionary<TAgent, TScreen>();
            _economyCount = new Dictionary<TScreen, int>();
            SetupScreens();
        }

        public int GetCount(TScreen screen)
        {
            if (_economyCount.ContainsKey(screen))
            {
                return _economyCount[screen];
            }
            return 0;
        }

        private void ChangeEconomyScreen(TAgent agent, TScreen newScreen)
        {
            var oldScreen = EconomyScreens[agent];
            EconomyScreens[agent] = newScreen;
            AddToEconomyCount(newScreen);
            RemoveEconomyCount(oldScreen);
        }

        private void AddToEconomyCount(TScreen screen)
        {
            if (!_economyCount.ContainsKey(screen))
            {
                _economyCount.Add(screen, 1);
            }
            else
            {
                _economyCount[screen]++;
            }
        }

        private void RemoveEconomyCount(TScreen screen)
        {
            if (_economyCount.ContainsKey(screen))
            {
                if (_economyCount[screen] > 0)
                {
                    _economyCount[screen]--;
                }
            }
        }

        public TScreen GetScreen(TAgent agent, TScreen defaultTScreen)
        {
            if (!EconomyScreens.ContainsKey(agent))
            {
                EconomyScreens.Add(agent, defaultTScreen);
                AddToEconomyCount(defaultTScreen);
            }
            return EconomyScreens[agent];
        }
        
        public void ChangeScreen(TAgent agent, TScreen choice)
        {
            var system = GetEconomySystem(agent);
            var canChange = system.CanMove(agent);
            if (canChange)
            {
                ChangeEconomyScreen(agent, choice);
            }
        }
    }
}
