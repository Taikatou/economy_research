using NUnit.Framework;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using EconomyProject.Scripts;
using EconomyProject.Scripts.UI;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Shop;

using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.GameEconomy.Systems.TravelSystem;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using TurnBased.Scripts;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////TO UNCOMMENT/////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
using EconomyProject.Scripts.GameEconomy.Systems.Requests;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;

using TurnBased.Scripts;
*/

namespace Tests.Economy
{
	public class AdventurerSystemTests
	{
		AdventurerSystem adventurerSystem;
		GetCurrentAdventurerAgent getAdventurerAgent;
		GetCurrentShopAgent getShopAgent;
		AdventurerAgent adventurerAgent;
		ShopAgent shopAgent;
		SystemSpawner systemSpawner;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////TO UNCOMMENT/////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//BattleSubSystem battleSubsystem;
		//RequestShopSystem requestShopSystem;

		[SetUp]
		public void Setup()
		{
			adventurerSystem = new AdventurerSystem();

			//Create 1 adventurer agent
			systemSpawner = Resources.FindObjectsOfTypeAll<SystemSpawner>()[0];
			systemSpawner.numLearningAgents = 1;
			systemSpawner.Start();
			getAdventurerAgent = Resources.FindObjectsOfTypeAll<GetCurrentAdventurerAgent>()[0];
			adventurerAgent = getAdventurerAgent.CurrentAgent;

			getShopAgent = Resources.FindObjectsOfTypeAll<GetCurrentShopAgent>()[0];
			shopAgent = getShopAgent.CurrentAgent;
		}

		

		/********************************************Travel System*********************************************/

		/// <summary>
		/// Test if an aventurer can travel to the 4 places : forest, mountain, sea, volcano 
		/// </summary>
		[Test]
		public void TestTravelSystem()
		{
			bool isForestExist = Enum.IsDefined(typeof(BattleEnvironments), "Forest");
			bool isMoutainExist = Enum.IsDefined(typeof(BattleEnvironments), "Mountain");
			bool isSeaExist = Enum.IsDefined(typeof(BattleEnvironments), "Sea");
			bool isVolcanoExist = Enum.IsDefined(typeof(BattleEnvironments), "Volcano");
			Assert.True(isForestExist && isMoutainExist && isSeaExist && isVolcanoExist == true);
		}

		/********************************************Battle System*********************************************/
		/// <summary>
		/// Test the existence of the adventures states : InBattle and OutOfBattle
		/// </summary>
		[Test]
		public void TestAdventureStates()
		{
			bool isInBattleExist = Enum.IsDefined(typeof(AdventureStates), "InBattle");
			bool isOutOfBattleExist = Enum.IsDefined(typeof(AdventureStates), "OutOfBattle");
			Assert.True(isInBattleExist && isOutOfBattleExist == true);
		}

		/// <summary>
		/// Test if there is a battle by default
		/// </summary>
		[Test]
		public void TestNoBattleByDefault()
		{
			Assert.False(adventurerSystem.GetBattleCount() != 0, "No battle by default");
		}

		/// <summary>
		/// Test if canmove=true by default
		/// </summary>
		[Test]
		public void TestCanMove()
		{
			Assert.True(adventurerSystem.CanMove(adventurerAgent) == true);
		}

		/// <summary>
		/// Test GetAdventureStates
		/// </summary>
		[Test]
		public void TestGetAdventureStates()
		{
			Assert.AreEqual(adventurerSystem.GetAdventureStates(adventurerAgent), AdventureStates.OutOfBattle);
		}

		/// <summary>
		/// Test GetSenses: must be [0,0,0,0,0] by default
		/// </summary>
		[Test]
		public void TestGetSensesOutBattle()
		{
			Assert.AreEqual(adventurerSystem.GetSenses(adventurerAgent), new float[5]);
		}

		/// <summary>
		/// Test having the BattleSubSystem from the agent
		/// </summary>
		[Test]
		public void TestGetSubsytem()
		{
			//Assert.IsNotNull(adventurerSystem.GetSubSystem(adventurerAgent));
		}

		/********************************************Wallet*********************************************/
		/// <summary>
		/// Test having the BattleSubSystem from the agent
		/// </summary>
		[Test]
		public void TestWallet()
		{
			adventurerAgent.wallet.Reset();
			Assert.AreEqual(adventurerAgent.wallet.startMoney, adventurerAgent.wallet.Money, "Start money should be 100");

			int money = adventurerAgent.wallet.Money;
			Assert.IsNotNull(money);

			adventurerAgent.wallet.EarnMoney(10);
			Assert.AreEqual(money + 10, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.LoseMoney(-20);
			Assert.AreEqual(money - 10, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.SpendMoney(30);
			Assert.AreEqual(money - 40, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.SetMoney(234);
			Assert.AreEqual(234, adventurerAgent.wallet.Money);

			adventurerAgent.wallet.LoseMoney(1000);
			Assert.AreEqual(0, adventurerAgent.wallet.Money);
		}


		
		/// <summary>
		/// Test
		/// </summary>
		
		public void TestBattleSystem()
		{
			
			TravelSubSystem ts = adventurerSystem.travelSubsystem;
			FighterObject aa = ts.GetBattle(BattleEnvironments.Forest);
			Assert.False( aa == null, " ");


			//battleSubSystem = adventurerSuystem.GetSubSystem(agent);


		
			//adventurerSystem.OnAttackButton(agent);
			//adventurerSystem.OnHealButton(agent);
			//adventurerSystem.OnFleeButton(agent);
		

			Assert.True(true);
		}


		/// <summary>
		/// Test taking random request
		/// </summary>
		
		public void TestSetupRequestSystem()
		{
			//Check types of requests
			bool isSetInputExist = Enum.IsDefined(typeof(RequestActions), "SetInput");
			bool isRemoveRequestExist = Enum.IsDefined(typeof(RequestActions), "RemoveRequest");
			bool isIncreasePriceExist = Enum.IsDefined(typeof(RequestActions), "IncreasePrice");
			bool isDecreasePriceExist = Enum.IsDefined(typeof(RequestActions), "DecreasePrice");
			Assert.False(isSetInputExist && isRemoveRequestExist && isIncreasePriceExist && isDecreasePriceExist == false, "Enumeration RequestActions wrong");

			//Ressources
			bool isNothingExist = Enum.IsDefined(typeof(CraftingResources), "Nothing");
			bool isWoodExist = Enum.IsDefined(typeof(CraftingResources), "Wood");
			bool isMetalExist = Enum.IsDefined(typeof(CraftingResources), "Metal");
			bool isGemExist = Enum.IsDefined(typeof(CraftingResources), "Gem");
			bool isDragonScaleExist = Enum.IsDefined(typeof(CraftingResources), "DragonScale");
			Assert.False(isNothingExist && isWoodExist && isMetalExist && isGemExist && isDragonScaleExist == false, "Enumeration CraftingResources wrong");

			//Empty list of request by default
			int countRequests = requestShopSystem.requestSystem.GetAllCraftingRequests().Count;
			Assert.False(countRequests != 0, "Some request(s) are already made by default");
		}


		/// <summary>
		/// Test taking random request
		/// </summary>
		
		public void TestTakeRequest()
		{
			//Make a request
			CraftingResources craftResourceToRequest = CraftingResources.Wood;
			requestShopSystem.MakeChoice(shopAgent, craftResourceToRequest);

			//Check the request
			int countRequests = requestShopSystem.requestSystem.GetAllCraftingRequests().Count;
			Assert.False(countRequests != 1, "A request should be created");

			CraftingResourceRequest requestMade = requestShopSystem.requestSystem.GetAllCraftingRequests()[0];
			Assert.False(requestMade == null, "CraftingResourceRequest empty");

			//Take a request
			requestShopSystem.requestSystem.TakeRequest(adventurerAgent.requestTaker, requestMade);

			//Check the taken quest
			List<CraftingResourceRequest> takenQuests = adventurerAgent.requestTaker.GetItemList();
			Assert.False(takenQuests.Count != 1, "The adventurer should have one request");

			//Accomplish request
		}



		/// <summary>
		/// Test buying ans equipping equipments
		/// </summary>
		
		public void TestBuyEquipment()
		{
			Assert.True(true);
		}



		*/
	}
}
