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
	public enum EShopAgentChoices
	{
		None,
		IncreasePrice,
		DecreasePrice,
		Up,
		Down,
		Select,
		RequestNone,
		RequestIncreasePrice,
		RequestDecreasePrice,
		RequestUp,
		RequestDown,
		RequestSelect,
		RemoveRequest,
		Back
	}

	public enum EShopScreen { Main, Request, Craft}
	
	public enum ENewShopScreen { Main }
	
	public enum EGoalSignal { MakeRequests, CreateItem, EditShop, FreeRoam }
    
	public class ShopAgent : AgentScreen, IEconomyAgent, IScreenSelect<ENewShopScreen>
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
			
			//var num = Enum.GetValues(typeof(EGoalSignal)).Cast<EGoalSignal>().Count();
			//vectorSensor.GetSensor().AddOneHotObservation((int) goalSignal, num);
		}

		public ENewShopScreen ChosenScreen
        {
            get
            {
	            var toReturn = ENewShopScreen.Main;

	            return toReturn;
            }
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            
            agentInventory.Setup();
            craftingInventory.ResetInventory();
            agentInventory.Setup();
            wallet.Setup(shopInput.requestSystem.system.requestSystem, AgentType.Shop);
            
            ResetOnItem.bSetupSystems = true;
        }

        public ShopHeuristic shopHeuristic;
        public bool controlWithHeuristic;

        public override void Heuristic(in ActionBuffers actionsOut)
        {
	        if (_forcedAction != EShopAgentChoices.None)
	        {
		        var actions = actionsOut.DiscreteActions;
		        if (_forcedAction < EShopAgentChoices.RequestIncreasePrice)
		        {
			        actions[0] = (int)_forcedAction;
		        }
		        else
		        {
			        actions[1] = (int)ChoicesMap[_forcedAction];
		        }

		        _forcedAction = EShopAgentChoices.None;
	        }
        }

        public static Dictionary<EShopAgentChoices, EShopAgentChoices> ChoicesMap = new Dictionary<EShopAgentChoices, EShopAgentChoices>
        {
	        { EShopAgentChoices.RemoveRequest, EShopAgentChoices.RemoveRequest },
	        { EShopAgentChoices.RequestUp, EShopAgentChoices.Up },
	        { EShopAgentChoices.RequestDown, EShopAgentChoices.Down },
	        { EShopAgentChoices.RequestSelect, EShopAgentChoices.Select },
	        { EShopAgentChoices.RequestIncreasePrice, EShopAgentChoices.IncreasePrice },
	        { EShopAgentChoices.RequestDecreasePrice, EShopAgentChoices.DecreasePrice },
        };

        public override void OnActionReceived(ActionBuffers actions)
        {
	        var battleAction = (EShopAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);
	        if (battleAction != EShopAgentChoices.None)
	        {
		        shopInput.shopCraftingSystem.system.AgentSetChoice(this, battleAction, 0);
	        }
	        var requestAction = (EShopAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[1]);
	        if (requestAction != EShopAgentChoices.None)
	        {
		        shopInput.requestSystem.system.AgentSetChoice(this, requestAction, 1);
	        }
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

		public IEnumerable<EnabledInput> GetEnabledInput()
		{
			throw new NotImplementedException();
		}

		public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
		{
			var i = 0;
			foreach (var sysInputs in GetEnabledInputNew())
			{
				foreach (var input in sysInputs)
				{
					actionMask.SetActionEnabled(i, input.Input, input.Enabled);
				}
				i++;
			}
		}

		public List<EnabledInput[]> GetEnabledInputNew()
		{
			var craftInputs = shopInput.shopCraftingSystem.system.GetEnabledInputs(this, 0);
			var requestInputs = shopInput.requestSystem.system.GetEnabledInputs(this, 1);
			return new List<EnabledInput[]>()
			{
				craftInputs,
				requestInputs
			};
		}

		public int HalfSize => 6;
	}
}
