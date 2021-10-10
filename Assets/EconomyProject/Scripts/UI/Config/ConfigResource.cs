using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigResource : AbstractConfigComponent<Dictionary<ECraftingResources, int>, ECraftingResources>
	{
		public override void SetupAbstractComponent()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigResources>();
		}

		public new void Setup(ECraftingResources newItem, int newPrice)
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