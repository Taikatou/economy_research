using EconomyProject.Monobehaviours;
using EconomyProject.Scripts;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using Inventory;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentReset : MonoBehaviour
{
	protected GetCurrentAdventurerAgent getCurrentAdventurerAgent;
	protected GetCurrentShopAgent getCurrentShopAgent;
	public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
	public RequestShopSystemBehaviour requestShopSystemBehaviour;
	public RequestAdventurerSystemBehaviour requestAdventurerSystemBehaviour;
	public AdventurerSystemBehaviour adventurerSystemBehaviour;
	public ConfigSystem configSystem;

	public GameObject UIBlocker;
	public GameObject ConfigUI;

	public void Start()
	{
		getCurrentAdventurerAgent = FindObjectOfType<GetCurrentAdventurerAgent>();
		getCurrentShopAgent = FindObjectOfType<GetCurrentShopAgent>();
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
			//agent.ResetEconomyAgent();
			agent.OnEpisodeBegin();
		}

		foreach (var agent in getCurrentShopAgent.GetAgents)
		{
			//agent.ResetEconomyAgent();
			agent.OnEpisodeBegin();
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
			agent.SetAction(EShopAgentChoices.None);
		}
	}

	/// <summary>
	/// Reset Config changements
	/// </summary>
	public void ResetConfig()
	{
		return;
		//Make the default price 
		List<BaseItemPrices> defaultPrices = new List<BaseItemPrices>();
		var basePrices = shopCraftingSystemBehaviour.system.shopSubSubSystem.basePrices;
		foreach (BaseItemPrices itemPrice in basePrices)
		{
			UsableItem newItem = itemPrice.item;
			int newPrice = 0;
			switch (itemPrice.item.ToString())
			{
				case "Beginner Sword":
					newPrice = 50;
					break;
				case "Intermediate Sword":
					newPrice = 70;
					break;
				case "Advanced Sword":
					newPrice = 90;
					break;
				case "Epic Sword":
					newPrice = 110;
					break;
				case "Master Sword":
					newPrice = 140;
					break;
				case "Ultimate Sword":
					newPrice = 180;
					break;
				default:
					Debug.Log("Wrong name : " + itemPrice.item.ToString());
					break;
			}
			defaultPrices.Add(new BaseItemPrices
			{
				item = newItem,
				price = newPrice
			});
		}

		//Reset config
		configSystem.SetDefaultItemDetails(ItemData.GetDefaultDurabilities(), ItemData.GetDefaultDamages());
		configSystem.SetItemsDefaultPrices(defaultPrices);
	}
}
