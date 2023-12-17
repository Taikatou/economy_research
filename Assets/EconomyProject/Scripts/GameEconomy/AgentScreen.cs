using System;
using EconomyProject.Scripts.GameEconomy.Systems;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public interface IScreenSelect<out T> where T : Enum
    {
        public T ChosenScreen { get; }
    }
	public enum AgentType { Adventurer, Shop, None }

	public abstract class AgentScreen : Agent
    {
        public abstract int ChosenScreenInt { get; }
		public abstract AgentType agentType { get; }

        private int _previousDown = -1;

        private readonly KeyCode[] _keyCodes = {
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        
        protected int NumberKey
        {
            get
            {
                var found = false;
                var action = -1;
                for(var i = 0 ; i < _keyCodes.Length && !found; i ++ )
                {
                    if (Input.GetKeyDown(_keyCodes[i]) || Input.GetKey(_keyCodes[i]))
                    {
                        if (_previousDown != i)
                        {
                            action = i;
                            _previousDown = i;
                        }
                        found = true;
                    }
                }

                if (!found)
                {
                    _previousDown = -1;
                }
                
                return action;   
            }
        }
        
        protected int GetAction(EnabledInput[] choices, int maxValue)
        {
            var random = new System.Random();
            var chosenAction = 0;
            do
            {
                chosenAction = random.Next(0, maxValue);
            } while (!choices[chosenAction].Enabled);

            return choices[chosenAction].Input;
        }
    }
}
