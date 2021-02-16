using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using UnityEngine;

public class EnvironmentReset : MonoBehaviour
{
	protected GetCurrentAdventurerAgent getCurrentAdventurerAgent;
	protected GetCurrentShopAgent getCurrentShopAgent;
	public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
	public RequestShopSystemBehaviour requestShopSystemBehaviour;
	public RequestAdventurerSystemBehaviour requestAdventurerSystemBehaviour;
	public AdventurerSystemBehaviour adventurerSystemBehaviour;

	public GameObject UIBlocker;
	public GameObject ConfigUI;

	void Start()
	{
		getCurrentAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
		getCurrentShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
	}

	public void ResetScript()
	{
		Debug.Log("RESET");

		UIBlocker.SetActive(true);
		ConfigUI.SetActive(true);

		ResetBattle();
		ResetShop();
		ResetRequest();
		ResetAgents();
		GoBackToMainMenu();
	}

	/// <summary>
	/// Reset battles
	/// </summary>
	public void ResetBattle()
	{
		adventurerSystemBehaviour.system.ResetAdventurerSystem();
	}

	/// <summary>
	/// Reset all the shop requests
	/// </summary>
	public void ResetShop()
	{
		shopCraftingSystemBehaviour.system.shopSubSubSystem.ResetShop();
		shopCraftingSystemBehaviour.system.craftingSubSubSystem.ResetCraftingSubSystem();

		shopCraftingSystemBehaviour.system.shopSubSubSystem.Refresh();
	}

	/// <summary>
	/// Reset all the resource requests taken and the resource requests made
	/// </summary>
	public void ResetRequest()
	{
		requestAdventurerSystemBehaviour.system.requestSystem.ResetRequestSystem();
		requestShopSystemBehaviour.system.requestSystem.ResetRequestSystem();

		requestAdventurerSystemBehaviour.system.requestSystem.craftingRequestRecord.ResetCraftingRequestRecord();
		requestShopSystemBehaviour.system.requestSystem.craftingRequestRecord.ResetCraftingRequestRecord();

		requestAdventurerSystemBehaviour.system.requestSystem.Refresh();
		requestShopSystemBehaviour.system.requestSystem.Refresh();
	}

	/// <summary>
	/// Reset wallets + inventories of all agents
	/// </summary>
	public void ResetAgents()
	{
		foreach (var agent in getCurrentAdventurerAgent.GetAgents)
		{
			agent.ResetEconomyAgent();
		}

		foreach (var agent in getCurrentShopAgent.GetAgents)
		{
			agent.ResetEconomyAgent();
		}
	}

	/// <summary>
	/// Get all the agents to the main menu
	/// </summary>
	public void GoBackToMainMenu()
	{
		foreach (var agent in getCurrentAdventurerAgent.GetAgents)
		{
			//Not working if the adventurer is in a battle
			//agent.SetAction(EAdventurerAgentChoices.MainMenu);
		}

		foreach (var agent in getCurrentShopAgent.GetAgents)
		{
			agent.SetAction(EShopAgentChoices.MainMenu);
		}
	}
}
