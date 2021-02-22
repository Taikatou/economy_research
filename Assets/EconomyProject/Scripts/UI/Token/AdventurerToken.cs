using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.UI.Token
{
	public class AdventurerToken : AgentToken<AdventurerAgent, EAdventurerAgentChoices>
	{
		public Transform arenaForest;
		public Transform arenaMountain;
		public Transform arenaSea;
		public Transform arenaVolcano;

		void Update()
		{
			//TODELETE
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				this.transform.position = agentList.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.transform.position = shop.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.transform.position = request.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.transform.position = arenaForest.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.transform.position = arenaMountain.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				this.transform.position = arenaSea.position;
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				this.transform.position = arenaVolcano.position;
			}


			/*****************************************************************************/



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
		void UpdateTokenPosition(EAdventurerAgentChoices choice)
		{
			switch (choice)
			{
				case EAdventurerAgentChoices.None:
					this.transform.position = agentList.position;
					break;
				case EAdventurerAgentChoices.MainMenu:
					this.transform.position = agentList.position;
					break;
				case EAdventurerAgentChoices.Shop:
					this.transform.position = shop.position;
					break;
				case EAdventurerAgentChoices.PurchaseItem:
					//Animation ?
					this.transform.position = shop.position;
					break;
				case EAdventurerAgentChoices.ResourceRequest:
					this.transform.position = request.position;
					break;
				case EAdventurerAgentChoices.TakeResourceRequest:
					//Animation ?
					this.transform.position = request.position;
					break;
				case EAdventurerAgentChoices.AdventureForest:
					this.transform.position = arenaForest.position;
					break;
				case EAdventurerAgentChoices.AdventureMountain:
					this.transform.position = arenaMountain.position;
					break;
				case EAdventurerAgentChoices.AdventureSea:
					this.transform.position = arenaSea.position;
					break;
				case EAdventurerAgentChoices.AdventureVolcano:
					this.transform.position = arenaVolcano.position;
					break;
				default:
					this.transform.position = agentList.position;
					Debug.LogWarning("Wrong choice : " + choice);
					break;
			}
		}
	}
}
