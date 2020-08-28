using System;
using System.Collections.Generic;
using System.Linq;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public struct InputAction
    {
        public string Action;
        public int ActionNumber;
    }

    public static class EconomySystemUtils
    {
        public static List<InputAction> GetStateInput<TG>() where TG : Enum
        {
            var values = Enum.GetValues(typeof(TG)).Cast<TG>();
            return GetStateInput<TG>(values);
        }
        
        public static List<InputAction> GetStateInput<TG>(IEnumerable<TG> values) where TG : Enum
        {
            var output = new List<InputAction>();
            foreach (var v in values)
            {
                output.Add(new InputAction
                {
                    Action = v.ToString(),
                    ActionNumber = Convert.ToInt32(v)
                });
            }
            return output;
        }
    }
    
    
    public abstract class EconomySystem<TAgent, TScreen> : MonoBehaviour where TAgent : AgentScreen<TScreen> where TScreen : Enum
    {
        public GameObject agents;
        public AgentInput<TAgent, TScreen> AgentInput { get; set; }
        public abstract TScreen ActionChoice { get; }

        public abstract bool CanMove(TAgent agent);

        public abstract float [] GetSenses(TAgent agent);

        public abstract InputAction[] GetInputOptions(TAgent agent);

        private Dictionary<TAgent, DateTime> _refreshTime;
        
        public virtual void Start()
        {
            _refreshTime = new Dictionary<TAgent, DateTime>();
        }

        protected void Refresh(TAgent agent)
        {
            if (_refreshTime.ContainsKey(agent))
            {
                _refreshTime.Add(agent, DateTime.Now);
            }
            else
            {
                _refreshTime[agent] = DateTime.Now;
            }
        }

        public DateTime GetRefreshTime(TAgent agent)
        {
            if (_refreshTime.ContainsKey(agent))
            {
                return _refreshTime[agent];
            }

            return DateTime.MinValue;
        }
        
        protected TAgent[] CurrentPlayers
        {
            get
            {
                var playerAgents = agents.GetComponentsInChildren<TAgent>();
                return Array.FindAll(playerAgents, element => element.ChosenScreen.Equals(ActionChoice));
            }
        }

        protected void RequestDecisions()
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
