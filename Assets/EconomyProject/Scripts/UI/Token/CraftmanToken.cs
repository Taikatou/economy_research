using EconomyProject.Scripts.MLAgents.Shop;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Token
{
	public class CraftmanToken : AgentToken<ShopAgent, EShopAgentChoices>
	{
		void Update()
		{
			if (agent.agentChoice == previousChoice)
			{
				return;
			}

			previousChoice = agent.agentChoice;

			UpdateTokenPosition(agent.agentChoice);
		}

		/// <summary>
		/// Move the token of the agent according to its action choice
		/// </summary>
		void UpdateTokenPosition(EShopAgentChoices choice)
		{
			switch (choice)
			{
				case EShopAgentChoices.None:
					this.transform.position = agentList.position;
					break;
				case EShopAgentChoices.MainMenu:
					this.transform.position = agentList.position;
					break;
				case EShopAgentChoices.Craft:
					this.transform.position = craftPlace.position;
					break;
				case EShopAgentChoices.CraftItem:
					//Animation?
					break;
				case EShopAgentChoices.SubmitToShop:
					this.transform.position = shop.position;
					break;
				case EShopAgentChoices.IncreasePrice:
					//Animation?
					this.transform.position = shop.position;
					break;
				case EShopAgentChoices.DecreasePrice:
					//Animation?
					this.transform.position = shop.position;
					break;
				case EShopAgentChoices.RequestResource:
					this.transform.position = request.position;
					break;
				case EShopAgentChoices.MakeResourceRequest:
					//Animation?
					this.transform.position = request.position;
					break;
				default:
					this.transform.position = agentList.position;
					Debug.LogWarning("Wrong choice : " + choice);
					break;
			}
		}
	}
}
