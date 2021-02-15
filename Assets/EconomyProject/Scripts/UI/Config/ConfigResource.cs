using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigResource : AbstractConfigComponent<Dictionary<CraftingResources, int>, CraftingResources>
	{
		public override void SetupAbstractComponent()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigResources>();
		}

		
		//Hide parent Setup() because has more field to fill in ConfigItem than the other components, then call the parent Setup function
		public new void Setup(CraftingResources newItem, int newPrice)
		{
			base.Setup(newItem, newPrice);
			SetupOthersParameters();
		}

		public void SetupOthersParameters()
		{
			image.sprite = GameObject.FindObjectOfType<ListConfigResources>().requestShopSystemBehaviour.system.requestSystem.GetIconByResource(item);
		}
	}
}