using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ListConfigAgents : AbstractList<Dictionary<AgentType, int>, AgentType>
	{
		public SystemSpawner adventurerSpawner;
		public SystemSpawner shopSpawner;
		public Sprite adventurerSprite;
		public Sprite shopAgentSprite;

		public Dictionary<AgentType, int> nbrAgents = new Dictionary<AgentType, int>();

		public override void SetupItems()
		{
			var requestSystem = FindObjectOfType<RequestSystem>();
			if (requestSystem != null)
			{
				_items = requestSystem.StartMoney;
				nbrAgents.Add(AgentType.Adventurer, adventurerSpawner.numLearningAgents);
				nbrAgents.Add(AgentType.Shop, shopSpawner.numLearningAgents);	
			}
		}

		public override void SetupList()
		{
			if(_items == null)
			{
				Debug.Log("ListAgent null");
				return;
			}

			foreach (var agent in _items)
			{
				GameObject newAgent = item;
				newAgent.GetComponent<ConfigAgent>().Setup(agent.Key, agent.Value);
				Instantiate(newAgent, holder.transform);
			}
		}

		public override void SetItem(AgentType nameAgent, int newValue, string category = null)
		{
			switch (category)
			{
				case null:
					if (_items.ContainsKey(nameAgent))
					{
						_items[nameAgent] = newValue;
					}
					else
					{
						Debug.LogWarning("Wrong key resource : " + nameAgent.ToString());
					}
					break;
				case "NbrAgent":
					if (nbrAgents.ContainsKey(nameAgent))
					{
						nbrAgents[nameAgent] = newValue;
					}
					else
					{
						Debug.LogWarning("Wrong key resource : " + nameAgent.ToString());
					}
					break;
			}
		}

		public Dictionary<AgentType, int> GetDefaultNbrAgents()
		{
			return nbrAgents;
		}
	}
}
