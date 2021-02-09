using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigResource : AbstractConfigComponent<Dictionary<CraftingResources, int>, CraftingResources>
	{
		public override void SetupAbstractList()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigResources>();
		}
	}
}