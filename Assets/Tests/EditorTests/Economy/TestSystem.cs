using System;
using System.Collections.Generic;
using EconomyProject;
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

		public ShopCraftingSystemBehaviour shopCraftingSystemBehaviour => GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>();
		public CraftingSubSystem craftingSubSystem => shopCraftingSystemBehaviour.system.craftingSubSubSystem;
		public AgentShopSubSystem agentShopSubSystem => shopCraftingSystemBehaviour.system.shopSubSubSystem;
		public GetCurrentShopAgent getShopAgent;
		public ShopAgent shopAgent;
		public AgentInventory shopAgentInventory;
		public ShopCraftingSystem shopCraftingSystem;

		public SystemSpawner adventurerSpawner;
		public SystemSpawner shopSpawner;

		public EnvironmentReset environmentReset;

		public ConfigSystem configSystem;

		public readonly List<EBattleEnvironments> ListEnvironments = new List<EBattleEnvironments> { EBattleEnvironments.Forest, EBattleEnvironments.Mountain, EBattleEnvironments.Sea, EBattleEnvironments.Volcano };
		public List<ECraftingResources> listCraftingResources = new List<ECraftingResources> { ECraftingResources.Wood, ECraftingResources.Metal, ECraftingResources.Gem, ECraftingResources.DragonScale };
		public List<ECraftingChoice> listCraftingChoices = new List<ECraftingChoice> { ECraftingChoice.BeginnerSword, ECraftingChoice.IntermediateSword, ECraftingChoice.AdvancedSword, ECraftingChoice.EpicSword, ECraftingChoice.UltimateSwordOfPower };

		public void Init()
		{
			adventurerSystemBehaviour = GameObject.FindObjectOfType<AdventurerSystemBehaviour>();
			adventurerSystem = adventurerSystemBehaviour.system;

			SpawnAgents();

			//Get Adventurer Agent
			getAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			//Generate travelSubsystem of the adventurerSystem
			travelSubsystem.Start();

			//Request System
			requestShopSystemBehaviour = GameObject.FindObjectOfType<RequestShopSystemBehaviour>();
			requestShopSystem = requestShopSystemBehaviour.system;
			requestSystem = requestShopSystem.requestSystem;
			requestSystem.Start();

			//Generate PlayerFighterData of the adventurerAgent
			adventurerAgent.OnEpisodeBegin();
			//Generate adventurerInventory of the adventurerAgent
			adventurerInventory = adventurerAgent.adventurerInventory;
			//Generate agentInventory of the adventurerAgent
			adventurerAgentInventory = adventurerAgent.inventory;
			adventurerAgent.OnEpisodeBegin();
			//Generate AdventurerRequestTaker
			adventurerAgent.requestTaker.Start();


			//Get ShopAgent
			getShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
			shopAgent = getShopAgent.CurrentAgent;
			shopAgent.shopInput.Awake();
			shopAgent.OnEpisodeBegin();
			shopAgent.craftingInventory.ResetInventory();

			//ShopSystem
			shopCraftingSystemBehaviour.Start();

			//ResetSystem
			environmentReset = GameObject.FindObjectOfType<EnvironmentReset>();
			environmentReset.Start();

			//Config
			configSystem = GameObject.FindObjectOfType<EconomyProject.Scripts.ConfigSystem>();
			configSystem.Start();
		}

		/********************************************Spawn*********************************************/

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
					shopAgentSpawned = true;
					shopSpawner = spawner;
					shopSpawner.numLearningAgents = 1;
					shopSpawner.StartSpawn();
				}
				if (spawner.name == "AdventurerGameSystem")
				{
					adventurerAgentSpawned = true;
					adventurerSpawner = spawner;
					adventurerSpawner.numLearningAgents = 1;
					adventurerSpawner.StartSpawn();

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

		public void DestroyAgents()
		{
			//Delete previous agents
			getAdventurerAgent.ClearGetAgents();
			getShopAgent.ClearGetAgents();
		}

		/********************************************Battle*********************************************/

		/// <summary>
		/// Start a battle and return the BattleSubSystem associated to this battle
		/// </summary>
		public BattleSubSystemInstance<AdventurerAgent> StartBattle(EBattleEnvironments env)
		{
			//TODO adventurerSystem.StartBattle(adventurerAgent, env);
			return adventurerSystem.BattleSubSystem.GetSubSystem(adventurerAgent);
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
		public UsableItem GetUsableItemByCraftingChoice(ECraftingChoice craftingChoice)
		{
			String nameToCheck = "";
			switch (craftingChoice)
			{
				case ECraftingChoice.BeginnerSword:
					nameToCheck = "Beginner Sword";
					break;
				case ECraftingChoice.IntermediateSword:
					nameToCheck = "Intermediate Sword";
					break;
				case ECraftingChoice.AdvancedSword:
					nameToCheck = "Advanced Sword";
					break;
				case ECraftingChoice.EpicSword:
					nameToCheck = "Epic Sword";
					break;
				case ECraftingChoice.MasterSword:
					nameToCheck = "Master Sword";
					break;
				case ECraftingChoice.UltimateSwordOfPower:
					nameToCheck = "Ultimate Sword";
					break;
				default:
					Debug.Log("Wrong craftingChoice : " + craftingChoice);
					break;
			}

			var basePrices = agentShopSubSystem.basePrices;
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
		public UsableItem AddItemInInventory(AgentInventory inventory, ECraftingChoice choice)
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
			shopAgent.craftingInventory.AddResource(ECraftingResources.Wood, 20);
			shopAgent.craftingInventory.AddResource(ECraftingResources.Metal, 20);
			shopAgent.craftingInventory.AddResource(ECraftingResources.Gem, 20);
			shopAgent.craftingInventory.AddResource(ECraftingResources.DragonScale, 20);
		}

		/********************************************TearDown*********************************************/

		public void TearDown()
		{
			//Reset
			environmentReset.ResetScript();
			environmentReset.ResetConfig();

			//Destroy agents
			DestroyAgents();
		}
	}
}