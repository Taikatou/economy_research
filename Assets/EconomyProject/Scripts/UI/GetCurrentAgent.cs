using System;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.MLAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
    public abstract class GetCurrentAgent<TAgent> : LastUpdate where TAgent : Agent
    {
        public GameObject agentParent;
        private int Index { get; set; }
        public TAgent[] GetAgents => agentParent.GetComponentsInChildren<TAgent>();

        public AgentSpawner agentSpawner;

        private DateTime _updateTime;

        public void Update()
        {
            Debug.Log(agentSpawner);
            if (_updateTime != agentSpawner.LastUpdated)
            {
                _updateTime = agentSpawner.LastUpdated;
                Refresh();
            }
        }

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
