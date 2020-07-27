using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy
{
    public abstract class AgentScreen<TScreen> : Agent
    {
        public abstract TScreen ChosenScreen { get; }
        
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
                var action = -1;
                for(var i = 0 ; i < _keyCodes.Length; i ++ )
                {
                    if (Input.GetKeyDown(_keyCodes[i]) || Input.GetKey(_keyCodes[i]))
                    {
                        action = i;
                        break;
                    }
                }
                Debug.Log(action);
                return action;   
            }
        }
    }
}
