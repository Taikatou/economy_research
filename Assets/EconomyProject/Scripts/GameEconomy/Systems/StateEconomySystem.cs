using System;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    public abstract class StateEconomySystem<TState, TAgent, TScreen> : EconomySystem<TAgent, TScreen> where TState : Enum where TAgent : AgentScreen<TScreen> where TScreen : Enum
    {
        private Dictionary<TAgent, TState> _inputMode;
        
        protected abstract TState IsBackState { get; }

        protected abstract TState DefaultState { get; }

        protected abstract void MakeChoice(TAgent agent, int input);

        public virtual void Start()
        {
            _inputMode = new Dictionary<TAgent, TState>();
        }
        
        private static T ToEnum<T>(int value)
        {
            var type = typeof(T);
         
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{type} is not an enum.");
            }
            if (!type.IsEnumDefined(value))
            { 
                throw new ArgumentException($"{value} is not a valid ordinal of type {type}."); 
            }
            return (T)Enum.ToObject(type, value); 
        }
        
        public override void SetChoice(TAgent agent, int input)
        {
            if (input >= 0)
            {
                if (Enum.IsDefined(typeof(TState), input))
                {
                    SetInputMode(agent, ToEnum<TState>(input));
                }
                else
                {
                    MakeChoice(agent, input);
                }
            }
        }

        public TState GetInputMode(TAgent agent)
        {
            if (_inputMode.ContainsKey(agent))
            {
                return _inputMode[agent];
            }

            return DefaultState;
        }

        protected void SetInputMode(TAgent agent, TState state)
        {
            if (_inputMode.ContainsKey(agent))
            {
                _inputMode[agent] = state;
            }
            else
            {
                _inputMode.Add(agent, state);
            }

            CheckBack(agent, state);
            Debug.Log("Set state: " + state);
        }

        private void CheckBack(TAgent agent, TState state)
        {
            if (state.Equals(IsBackState))
            {
                GoBack(agent);
            }
        }

        protected abstract void GoBack(TAgent agent);
    }
}
