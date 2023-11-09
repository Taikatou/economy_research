using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.MLAgents.Shop
{
	public enum EShopAgentChoices { Back, IncreasePrice, DecreasePrice, Up, Down, Select, RemoveRequest, None }

	public enum EShopScreen { Main, Request, Craft}
	
	public enum EGoalSignal { MakeRequests, CreateItem, EditShop, FreeRoam }
    
	public class ShopAgent : AgentScreen, IEconomyAgent, IScreenSelect<EShopScreen>
	{
		public VectorSensorComponent vectorSensor;
	    public ShopInput shopInput;
        public EconomyWallet wallet;
        public CraftingInventory craftingInventory;
        public AgentInventory agentInventory;

        private EShopAgentChoices _forcedAction;
        private bool _bForcedAction;
        public override int ChosenScreenInt => (int) ChosenScreen;
        public override AgentType agentType => AgentType.Shop;

		public void Start()
		{
			wallet.onEarnMoney = OnEarnMoney;
			agentInventory.onItemAdd += OnCraft;
		}

		private void OnEarnMoney(float amount)
		{
			if (TrainingConfig.OnSell)
			{
				AddReward(TrainingConfig.OnSellReward);
			}
		}

		private void OnCraft(UsableItem usableItem)
		{
			if (TrainingConfig.OnCraft)
			{
				AddReward(TrainingConfig.OnCraftReward);
			}
		}

		public override void CollectObservations(VectorSensor sensor)
		{
			EGoalSignal goalSignal;
			if (craftingInventory.GetResourceNumber() == 0)
			{
				goalSignal = EGoalSignal.MakeRequests;
			}
			else if (agentInventory.NumItems() == 0)
			{
				goalSignal = EGoalSignal.CreateItem;
			}
			else if (shopInput.shopCraftingSystem.system.shopSubSubSystem.GetShopItems(this).Count == 0)
			{
				goalSignal = EGoalSignal.EditShop;
			}
			else
			{
				goalSignal = EGoalSignal.FreeRoam;
			}
			
			var num = Enum.GetValues(typeof(EGoalSignal)).Cast<EGoalSignal>().Count();
			vectorSensor.GetSensor().AddOneHotObservation((int) goalSignal, num);
		}

		public EShopScreen ChosenScreen
        {
            get
            {
	            var toReturn = EShopScreen.Main;
	            if (shopInput != null)
	            {
		            toReturn = shopInput.GetScreen(this, EShopScreen.Main);
	            }
                
                return toReturn;
            }
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            
            agentInventory.Setup();
            craftingInventory.ResetInventory();
            agentInventory.Setup();
            wallet.Setup(shopInput.requestSystem.system.requestSystem, AgentType.Adventurer);
            
            ResetOnItem.bSetupSystems = true;
        }

        public ShopHeuristic shopHeuristic;
        public bool controlWithHeuristic;

        public override void Heuristic(in ActionBuffers actionsOut)
        {
	        if (controlWithHeuristic)
	        {
		        shopHeuristic.Heuristic(shopInput.requestSystem.system.requestSystem, shopInput.shopCraftingSystem.system, this);
	        }
	        else
	        {
		        var actionB = actionsOut.DiscreteActions;
		        actionB[0] = (int)EShopAgentChoices.None;
	        }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
	        var action = (EShopAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);;
	        if (_bForcedAction)
	        {
		        _bForcedAction = false;
		        action = _forcedAction;
	        }
	        
	        var system = shopInput.GetEconomySystem(this);
            system.AgentSetChoice(this, action);
        }

        public void SetAction(EShopAgentChoices choice)
		{
			_bForcedAction = true;
			_forcedAction = choice;
		}

		public void SetAction(int action)
		{
			SetAction((EShopAgentChoices) action);
		}
		
		public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
		{
			foreach (var input in GetEnabledInput())
			{
				actionMask.SetActionEnabled(0, input.Input, input.Enabled);   
			}
		}

		public IEnumerable<EnabledInput> GetEnabledInput()
		{
			return shopInput.GetActionMask(this);
		}
    }
}
