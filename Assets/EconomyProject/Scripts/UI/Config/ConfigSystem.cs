using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.UI.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EconomyProject.Scripts
{
	public class ConfigSystem : MonoBehaviour
	{
		protected ShopCraftingSystemBehaviour shopCraftingSystemBehaviour;
		protected ShopCraftingSystem shopCraftingSystem;
		protected AgentShopSubSystem agentShopSubSystem;
		protected GetCurrentShopAgent getCurrentShopAgent;
		protected GetCurrentAdventurerAgent getCurrentAdventurerAgent;

		protected RequestShopSystemBehaviour requestShopSystemBehaviour;

		public ListConfigItems listConfigItems;
		public ListConfigResources listConfigResources;
		public ListConfigAgents listConfigAgents;
		

		private void Start()
		{
			//TO DELETE WHEN IT WILL BE ATTACHED TO A GAMEOBJECT
			shopCraftingSystemBehaviour = GameObject.FindObjectOfType<ShopCraftingSystemBehaviour>();
			shopCraftingSystem = shopCraftingSystemBehaviour.system;
			agentShopSubSystem = shopCraftingSystem.shopSubSubSystem;

			requestShopSystemBehaviour = GameObject.FindObjectOfType<RequestShopSystemBehaviour>();

			getCurrentShopAgent = GameObject.FindObjectOfType<GetCurrentShopAgent>();
			getCurrentAdventurerAgent = GameObject.FindObjectOfType<GetCurrentAdventurerAgent>();
		}

		public void SetItemsDefaultPrices(List<BaseItemPrices> listPrice)
		{
			//Modify the shopSystem
			agentShopSubSystem.basePrices = listPrice;

			//Modify in the shop agent
			ShopAgent[] shopAgents = getCurrentShopAgent.GetAgents;
			foreach(ShopAgent agent in shopAgents)
			{
				agentShopSubSystem.SetShop(agent, listPrice); //WARNING : This reset the stockprices. Talk with Conor if it's the expected behaviour
			}

			//+To test
		}

		public void SetResourceDefaultPrices(Dictionary<CraftingResources, int> newResourcePrices)
		{
			requestShopSystemBehaviour.system.requestSystem.defaultResourcePrices = newResourcePrices;

			//+ test
		}

		public void SetStartMoneyAgents(Dictionary<string, int> newStartMoney)
		{
			// create Dictionary<string, int> in economy wallet system or smth like that
		}

		/* Instead of doing that, do resetWallet
		public void StartMoneyActiveShopAgents(int newStartMoney)
		{
			//Should have an external array with the start money per agents 
			//_startMoney = { Adventurer = 100 , Shop = 1000};


			ShopAgent[] shopAgents = getCurrentShopAgent.GetAgents;
			foreach (ShopAgent agent in shopAgents)
			{
				agent.wallet.startMoney = newStartMoney;
				agent.wallet.SetMoney(newStartMoney);
			}

			//test
		}

		public void StartMoneyActiveAdventurerAgents(int newStartMoney)
		{
			AdventurerAgent[] shopAgents = getCurrentAdventurerAgent.GetAgents;
			foreach (AdventurerAgent agent in shopAgents)
			{
				agent.wallet.startMoney = newStartMoney;
				agent.wallet.SetMoney(newStartMoney);
			}

			//test
		}
		*/


		
		/// <summary>
		/// Apply all the default parameters to the system
		/// 1 - Item prices
		/// 2 - Resource prices
		/// 3 - Agents start money
		/// </summary>
		public void StartButton()
		{
			//Item prices
			List<BaseItemPrices> newItemPrices = listConfigItems.GetParameters();
			//SetItemsDefaultPrices(newItemPrices);


			//Resource prices
			Dictionary<CraftingResources, int> newResourcePrices = listConfigResources.GetParameters();
			//SetResourceDefaultPrices(newResourcePrices);


			//Agents start money
			Dictionary<string, int> newStartMoney = listConfigAgents.GetParameters();
			//SetStartMoneyAgents(newStartMoney);

			/*
			foreach (BaseItemPrices item in newItemPrices)
			{
				Debug.Log(item.item.itemDetails.itemName + " - " + item.price);
			}
			foreach (var item in newResourcePrices)
			{
				Debug.Log(item.Key.ToString() + " - " + item.Value);
			}
			foreach (var item in newStartMoney)
			{
				Debug.Log(item.Key.ToString() + " - " + item.Value);
			}*/



			//If not first changements ==> Reset with  bool mustReset
			//Else ==> StartSimulation + Spawners
			//Takeoff UIBlocker

			// + add imgs resources, sword, agents
			//Refactoring list+config items
		}
	}
}

