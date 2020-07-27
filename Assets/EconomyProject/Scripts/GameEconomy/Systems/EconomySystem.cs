﻿using System;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public abstract class EconomySystem<TAgent, TScreen> : MonoBehaviour where TAgent : AgentScreen<TScreen> where TScreen : Enum
    {
        public GameObject agents;
        
        public AgentInput<TAgent, TScreen> playerInput;

        public ShopInput ShopInput { get; set; }
        protected abstract TScreen ActionChoice { get; }

        public abstract bool CanMove(TAgent agent);
        
        public virtual float Progress => 0.0f;

        public TAgent[] CurrentPlayers
        {
            get
            {
                var playerAgents = agents.GetComponentsInChildren<TAgent>();
                return Array.FindAll(playerAgents, element => element.ChosenScreen.Equals(ActionChoice));
            }
        }

        protected virtual void RequestDecisions()
        {
            foreach(var agent in CurrentPlayers)
            {
                agent.RequestDecision();
            }
        }

        public virtual void SetChoice(TAgent agent, int input)
        {
            
        }
    }
}
