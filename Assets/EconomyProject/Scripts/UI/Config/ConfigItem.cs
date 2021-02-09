using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigItem : AbstractConfigComponent<List<BaseItemPrices>, UsableItem>
	{
		public override void SetupAbstractList()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigItems>();
		}
	}
}
