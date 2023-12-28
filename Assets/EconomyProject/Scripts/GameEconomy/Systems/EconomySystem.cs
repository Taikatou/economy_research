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

        public void AgentSetChoice(TAgent agent, TInput input)
        {
            
        }
        
        public virtual TInput[] GetEnabledInputs(TAgent agent)
        {
            return new TInput [] {};
        }
    }
}
