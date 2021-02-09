using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ListConfigAgents : AbstractList<Dictionary<string, int>, string>
	{
		public override void SetupItems()
		{
			//To take from elsewhere after
			_items = new Dictionary<string, int>
			{
				{ "adventurer", 100 },
				{ "shop", 1000 }
			};
		}

		public override void SetupList()
		{
			foreach (var agent in _items)
			{
				GameObject newAgent = item;
				newAgent.GetComponent<ConfigAgent>().Setup(agent.Key, agent.Value);
				Instantiate(newAgent, holder.transform);
			}
		}

		public override void SetItem(string nameAgent, int newPrice)
		{
			if (_items.ContainsKey(nameAgent))
			{
				_items[nameAgent] = newPrice;
			}
			else
			{
				Debug.LogWarning("Wrong key resource : " + nameAgent.ToString());
			}
		}
	}
}
