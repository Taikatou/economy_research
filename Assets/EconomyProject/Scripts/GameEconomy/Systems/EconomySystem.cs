using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public struct InputAction
    {
        public string Action;
        public int ActionNumber;
    }
    
    public struct EnabledInput
    {
        public int Input;
        public bool Enabled;
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

    public abstract class EconomySystem<TAgent, TScreen, TInput> where TAgent : AgentScreen<TScreen> where TScreen : Enum where TInput : Enum
    {
        public GetAgents agents;
        private readonly Dictionary<TAgent, DateTime> _refreshTime;
        public AgentInput<TAgent, TScreen, TInput> AgentInput { get; set; }
        public abstract TScreen ActionChoice { get; }
        public abstract bool CanMove(TAgent agent);
        public abstract ObsData [] GetObservations(TAgent agent);

        protected EconomySystem()
        {
            _refreshTime = new Dictionary<TAgent, DateTime>();
        }

        protected void Refresh(TAgent agent)
        {
            if (!_refreshTime.ContainsKey(agent))
            {
                _refreshTime.Add(agent, DateTime.Now);
            }
            else
            {
                _refreshTime[agent] = DateTime.Now;
            }
        }
        
        private bool ValidInput(TAgent agent, int input)
        {
            var inputs = GetEnabledInputs(agent);
            return inputs.Any(x => x.Input == input && x.Enabled);
        }

        public bool ValidInput(TAgent agent, TInput input)
        {
            return ValidInput(agent, Convert.ToInt32(input));
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

        protected virtual void SetChoice(TAgent agent, TInput input)
        {
            
        }

        public void AgentSetChoice(TAgent agent, TInput input)
        {
            var inputInt = Convert.ToInt32(input);
            var enabledInputs = GetEnabledInputs(agent);
            if (Array.Exists(enabledInputs, e => e.Input == inputInt))
            {
                var i = enabledInputs.First(x => x.Input == inputInt);
                if (i.Enabled)
                {
                    SetChoice(agent, input);
                }
            }
        }

        public virtual EnabledInput[] GetEnabledInputs(TAgent agent)
        {
            return new EnabledInput[] { };
        }
    }
}
