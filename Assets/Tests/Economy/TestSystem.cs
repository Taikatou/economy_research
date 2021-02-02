using System;
using System.Collections.Generic;
using UnityEngine;

using EconomyProject.Monobehaviours;
using EconomyProject.Scripts;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using Inventory;
using TurnBased.Scripts;

namespace Tests.Economy
{
	public class TestSystem
	{
		public AdventurerSystemBehaviour adventurerSystemBehaviour;
		public AdventurerSystem adventurerSystem;
		public GetCurrentAdventurerAgent getAdventurerAgent;
		public AdventurerAgent adventurerAgent;

		public TravelSubSystem travelSubsystem;
		public AdventurerInventory adventurerInventory;
		public AgentInventory adventurerAgentInventory;

		public RequestShopSystemBehaviour requestShopSystemBehaviour;
		public RequestShopSystem requestShopSystem;
		public RequestSystem requestSystem;

		public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		public CraftingSubSystem craftingSubSystem;
		public AgentShopSubSystem agentShopSubSystem;
		public GetCurrentShopAgent getShopAgent;
		public ShopAgent shopAgent;
		public AgentInventory shopAgentInventory;
		public ShopCraftingSystem shopCraftingSystem;
		

		public List<BattleEnvironments> listEnvironments = new List<BattleEnvironments> { BattleEnvironments.Forest, BattleEnvironments.Mountain, BattleEnvironments.Sea, BattleEnvironments.Volcano };
		public List<CraftingResources> listCraftingResources = new List<CraftingResources> { CraftingResources.Wood, CraftingResources.Metal, CraftingResources.Gem, CraftingResources.DragonScale };
		public List<CraftingChoice> listCraftingChoices = new List<CraftingChoice> { CraftingChoice.BeginnerSword, CraftingChoice.IntermediateSword, CraftingChoice.AdvancedSword, CraftingChoice.EpicSword, CraftingChoice.UltimateSwordOfPower };

		public void Init()
		{
			adventurerSystemBehaviour = GameObject.FindObjectOfType<AdventurerSystemBehaviour>();
			adventurerSystem = adventurerSystemBehaviour.system;

			SpawnAgents();

			//Get Adventurer Agent
			getAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			//Generate travelSubsystem of the adventurerSystem
			travelSubsystem = adventurerSystem.travelSubsystem;
			travelSubsystem.Start();
			adventurerSystem.travelSubsystem = travelSubsystem;

			//Request System
			requestShopSystemBehaviour = GameObject.FindObjectOfType<RequestShopSystemBehaviour>();
			requestShopSystem = requestShopSystemBehaviour.system;
			requestSystem = requestShopSystem.requestSystem;
			requestSystem.Start();

			//Generate PlayerFighterData of the adventurerAgent
			adventurerAgent.gameObject.GetComponent<AdventurerFighterData>().Start();
			//Generate adventurerInventory of the adventurerAgent
			adventurerInventory = adventurerAgent.adventurerInventory;
			//Generate agentInventory of the adventurerAgent
			adventurerAgentInventory = adventurerAgent.inventory;
			adventurerAgent.ResetEconomyAgent();
			//Generate AdventurerRequestTaker
			adventurerAgent.requestTaker.Start();


			//Get ShopAgent
			getShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
			shopAgent = getShopAgent.CurrentAgent;
			shopAgent.shopInput.Awake();
			shopAgent.agentInventory.ResetInventory();
			shopAgent.craftingInventory.ResetInventory();
			shopAgent.wallet.Reset();

			shopCraftingSystemBehaviour = GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>();
			shopCraftingSystemBehaviour.Start();
			agentShopSubSystem = shopCraftingSystemBehaviour.system.shopSubSubSystem;
			craftingSubSystem = shopCraftingSystemBehaviour.system.craftingSubSubSystem;
		}


		/// <summary>
		/// Spawn one shop agent and one afdventurer agent
		/// </summary>
		public void SpawnAgents()
		{
			//Create 1 adventurer agent
			SystemSpawner[] systemSpawners = GameObject.FindObjectsOfType<SystemSpawner>();

			bool shopAgentSpawned = false;
			bool adventurerAgentSpawned = false;
			foreach (SystemSpawner spawner in systemSpawners)
			{
				if (spawner.name == "CraftShopSystem")
				{
					spawner.numLearningAgents = 1;
					spawner.Start();
					shopAgentSpawned = true;
				}
				if (spawner.name == "AdventurerGameSystem")
				{
					spawner.numLearningAgents = 1;
					spawner.Start();
					adventurerAgentSpawned = true;
				}
			}

			if (shopAgentSpawned == false)
			{
				Debug.Log("Not found the Shop Agent Spawner : " + systemSpawners.Length);
			}
			if (adventurerAgentSpawned == false)
			{
				Debug.Log("Not found the Adventurer Agent Spawner : " + systemSpawners.Length);
			}
		}

		/********************************************Battle*********************************************/

		/// <summary>
		/// Start a battle and return the BattleSubSystem associated to this battle
		/// </summary>
		public BattleSubSystem StartBattle(BattleEnvironments env)
		{
			adventurerSystem.StartBattle(adventurerAgent, env);
			return adventurerSystem.GetSubSystem(adventurerAgent);
		}

		/********************************************Adventurer*********************************************/

		/// <summary>
		/// Debug Adventurer Agents items in inventory
		/// </summary>
		public void DebugItemsInInventory(AdventurerAgent agent)
		{
			AgentInventory inventory = agent.inventory;

			Debug.Log("Number of Items : " + inventory.Items.Count);
			foreach (KeyValuePair<string, List<UsableItem>> item in inventory.Items)
			{
				Debug.Log("item : " + item.Key
					+ "\n item list count: " + item.Value.Count
					);

				foreach (UsableItem usableItem in item.Value)
				{
					Debug.Log("usableItem name : " + usableItem.itemDetails.itemName
					+ "\n baseDurability: " + usableItem.itemDetails.baseDurability
					+ "\n durability : " + usableItem.itemDetails.durability
					+ "\n damage : " + usableItem.itemDetails.damage
					+ "\n unBreakable : " + usableItem.itemDetails.unBreakable
					+ "\n Broken : " + usableItem.itemDetails.Broken
					);
				}
			}
		}

		/// <summary>
		/// Get sword UsableItem by name
		/// </summary>
		public UsableItem GetSwordByName(String swordName)
		{
			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;

			foreach (BaseItemPrices item in basePrices)
			{
				if (item.item.ToString() == swordName)
				{
					return item.item;
				}
			}

			Debug.LogWarning("Not found sword called : " + swordName);
			return null;
		}

		/********************************************Shop*********************************************/

		

		/// <summary>
		/// Return UsableItem with a CraftingChoice
		/// </summary>
		public UsableItem GetUsableItemByCraftingChoice(CraftingChoice craftingChoice)
		{
			String nameToCheck = "";
			switch (craftingChoice)
			{
				case CraftingChoice.BeginnerSword:
					nameToCheck = "Beginner Sword";
					break;
				case CraftingChoice.IntermediateSword:
					nameToCheck = "Intermediate Sword";
					break;
				case CraftingChoice.AdvancedSword:
					nameToCheck = "Advanced Sword";
					break;
				case CraftingChoice.EpicSword:
					nameToCheck = "Epic Sword";
					break;
				/*
				case :
					nameToCheck = "Master Sword";
					break;
				*/
				case CraftingChoice.UltimateSwordOfPower:
					nameToCheck = "Ultimate Sword";
					break;
				default:
					Debug.Log("Wrong craftingChoice : " + craftingChoice);
					break;
			}

			List<BaseItemPrices> basePrices = agentShopSubSystem.basePrices;
			foreach (BaseItemPrices item in basePrices)
			{
				if (item.item.itemDetails.itemName == nameToCheck)
				{
					return item.item;
				}
			}

			Debug.Log("Not found the item");
			return null;
		}

		/// <summary>
		/// Add an Item in the agent inventory
		/// </summary>
		public UsableItem AddItemInInventory(AgentInventory inventory, CraftingChoice choice)
		{
			UsableItem itemToAdd = GetUsableItemByCraftingChoice(choice);
			inventory.AddItem(itemToAdd);

			return itemToAdd;
		}

		/// <summary>
		/// Gy price by item name
		/// </summary>
		public int GetPriceByItemName(string itemName)
		{
			foreach (BaseItemPrices item in agentShopSubSystem.basePrices)
			{
				UsableItemDetails details = item.item.itemDetails;
				if (item.item.itemDetails.itemName == itemName)
				{
					return item.price;
				}
			}

			Debug.Log("Wrong Item : " + itemName);
			return 0;
		}

		/********************************************Craft*********************************************/
		/// <summary>
		/// Give resources to the ShopAgent
		/// </summary>
		public void GiveResources()
		{
			shopAgent.craftingInventory.AddResource(CraftingResources.Wood, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.Metal, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.Gem, 20);
			shopAgent.craftingInventory.AddResource(CraftingResources.DragonScale, 20);
		}
	}
}
