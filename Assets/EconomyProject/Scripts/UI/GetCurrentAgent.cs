using System;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public abstract class GetCurrentAgent<TAgent> : MonoBehaviour where TAgent : Agent
    {
        public GameObject agentParent;

        private int Index { get; set; }

        public TAgent[] GetAgents => agentParent.GetComponentsInChildren<TAgent>();

        public TAgent CurrentAgent
        {
            get
            {
                if (GetAgents.Length > Index)
                {
                    return GetAgents[Index];
                }
                return null;
            }
        }

        public void UpdateAgent(TAgent agent)
        {
            var index = Array.FindIndex(GetAgents, a => a == agent);
            if (index >= 0)
            {
                Index = index;
            }
        }
    }
}
