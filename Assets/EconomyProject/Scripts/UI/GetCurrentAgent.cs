using System;
using Data;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.MLAgents;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using Unity.MLAgents;
using UnityEditor;
using UnityEngine;

namespace EconomyProject.Scripts.UI
{
	public abstract class GetCurrentAgent<TAgent> : LastUpdate where TAgent : Agent
	{
		public GetAgents agentParent;
		public int Index { get; private set; }
		public TAgent[] GetAgents => agentParent.GetComponentsInChildren<TAgent>();

		public BaseAgentSpawner agentSpawner;

        private DateTime _updateTime;

		private int nbrAgent = 0;

        public void Update()
        {
	        if (agentParent == null || GetAgents == null)
	        {
		        return;
	        }
            if (_updateTime != agentSpawner.LastUpdated || nbrAgent != GetAgents.Length)
            {
				nbrAgent = GetAgents.Length;
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

		/// <summary>
		/// Delete all the agents in the agent list
		/// </summary>
		public void ClearGetAgents()
		{
			foreach (Transform child in agentParent.transform)
			{
				//In Editor mode for the unit tests
				if (!EditorApplication.isPlaying)
				{
					GameObject.DestroyImmediate(child.gameObject);
				}
				//In Play mode
				else
				{
					GameObject.Destroy(child.gameObject);
				}
			}
		}
	}
}
