using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigAgent : AbstractConfigComponent<Dictionary<string, int>, string>
	{
		public override void SetupAbstractList()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigAgents>();
		}
	}
}