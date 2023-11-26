using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

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
        public int Branch;
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


    public abstract class EconomySystem<TAgent, TScreen, TInput> where TAgent : AgentScreen where TScreen : Enum where TInput : Enum
    {
        public GetAgents agents;
        private readonly Dictionary<TAgent, DateTime> _refreshTime;
        public AgentInput<TAgent, TScreen, TInput> AgentInput { get; set; }
        public abstract TScreen ActionChoice { get; }
        public abstract bool CanMove(TAgent agent);
        public abstract ObsData [] GetObservations(TAgent agent, BufferSensorComponent[] bufferSensorComponent);

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
        
        private bool ValidInput(TAgent agent, int input, int branch)
        {
            var inputs = GetEnabledInputs(agent, branch);
            return inputs.Any(x => x.Input == input && x.Enabled);
        }

        public bool ValidInput(TAgent agent, TInput input, int branch)
        {
            return ValidInput(agent, Convert.ToInt32(input), branch);
        }

        public DateTime GetRefreshTime(TAgent agent)
        {
            if (_refreshTime.ContainsKey(agent))
            {
                return _refreshTime[agent];
            }

            return DateTime.MinValue;
        }

        protected virtual void SetChoice(TAgent agent, TInput input)
        {
            
        }

        public void AgentSetChoice(TAgent agent, TInput input, int branch)
        {
            var inputInt = Convert.ToInt32(input);
            var enabledInputs = GetEnabledInputs(agent, branch);
            if (Array.Exists(enabledInputs, e => e.Input == inputInt))
            {
                var i = enabledInputs.First(x => x.Input == inputInt);
                if (i.Enabled)
                {
                    SetChoice(agent, input);
                }
            }
        }

        public virtual EnabledInput[] GetEnabledInputs(Agent agent,  int branch)
        {
            return new EnabledInput[] { };
        }
        
        public virtual EnabledInput[] GetEnabledInputs(TAgent agent, int branch)
        {
            return GetEnabledInputs((Agent)agent, branch);
        }
    }
}
