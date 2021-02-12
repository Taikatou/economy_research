using EconomyProject.Scripts.GameEconomy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EconomyProject.Scripts.UI.Config
{
	public class ConfigAgent : AbstractConfigComponent<Dictionary<AgentType, int>, AgentType>
	{
		[Header("NbrAgents")]
		public int nbrAgent;
		public Text nbrAgentText;
		public InputField inputTextNbrAgent;

		public override void SetupAbstractComponent()
		{
			abstractList = GameObject.FindObjectOfType<ListConfigAgents>();
		}

		//Hide parent Setup() because has more field to fill in ConfigItem than the other components, then call the parent Setup function
		public new void Setup(AgentType newAgent, int newPrice)
		{
			base.Setup(newAgent, newPrice);
			SetupOthersParameters(newAgent);
		}

		public void SetupOthersParameters(AgentType agentType)
		{
			switch (agentType)
			{
				case AgentType.Adventurer:
					nbrAgent = GameObject.FindObjectOfType<ListConfigAgents>().GetComponent<ListConfigAgents>().adventurerSpawner.numLearningAgents;
					break;
				case AgentType.Shop:
					nbrAgent = GameObject.FindObjectOfType<ListConfigAgents>().GetComponent<ListConfigAgents>().shopSpawner.numLearningAgents;
					break;
				default:
					Debug.Log("Wrong Agentype : " + agentType);
					break;
			}
			

			nbrAgentText.text = nbrAgent.ToString();
		}

		/********************************************Nbr Agent**********************************/
		/// <summary>
		/// Increment nbrAgent by +1
		/// </summary>
		public void IncreaseNbrAgent()
		{
			nbrAgent++;
			SetNbrAgent();
		}

		/// <summary>
		/// Increment nbrAgent by -1. Minimum nbrAgent = 1
		/// </summary>
		public void DecreaseNbrAgent()
		{
			if (nbrAgent <= 1)
			{
				return;
			}

			nbrAgent--;
			SetNbrAgent();
		}

		/// <summary>
		/// Set nbrAgent in the inputField and call the ListConfigItems
		/// </summary>
		public void SetNbrAgentInputField()
		{
			if (inputTextNbrAgent.text == null || inputTextNbrAgent.text == "")
			{
				inputTextNbrAgent.text = nbrAgent.ToString();
				return;
			}
			else
			{
				nbrAgent = int.Parse(inputTextNbrAgent.text);
			}
			SetNbrAgent();
		}

		/// <summary>
		/// Set the number of agent to spawn
		/// </summary>
		public void SetNbrAgent()
		{
			inputTextNbrAgent.text = nbrAgent.ToString();
			abstractList.SetItem(item, nbrAgent, "NbrAgent");
		}
	}
}