using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.Experiments;
using EconomyProject.Scripts.GameEconomy;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Shop;
using EconomyProject.Scripts.Inventory;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman;
using UnityEngine;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

namespace EconomyProject.Scripts.MLAgents.Shop
{
	public enum EShopAgentChoices
	{
		None,
		SelectWood,
		SelectMetal,
		SelectGem,
		SelectDragonScale,
		
		RequestIncreaseWood,
		RequestIncreaseMetal,
		RequestIncreaseGem,
		RequestIncreaseDragonScale,
		
		RequestDecreaseWood,
		RequestDecreaseMetal,
		RequestDecreaseGem,
		RequestDecreaseDragonScale,
		
		RequestRemoveWood,
		RequestRemoveMetal,
		RequestRemoveGem,
		RequestRemoveDragonScale,
		
		CraftBeginnerSword,
		CraftIntermediateSword,
		CraftAdvancedSword,
		CraftMasterSword,
		CraftEpicSword,
		CraftUltimateSword,
		
		IncreasePriceBeginnerSword,
		IncreasePriceIntermediateSword,
		IncreasePriceAdvancedSword,
		IncreasePriceMasterSword,
		IncreasePriceEpicSword,
		IncreasePriceUltimateSword,
		
		DecreasePriceBeginnerSword,
		DecreasePriceIntermediateSword,
		DecreasePriceAdvancedSword,
		DecreasePriceMasterSword,
		DecreasePriceEpicSword,
		DecreasePriceUltimateSword
	}

	public enum EShopScreen { Main, Request, Craft}
	
	public enum ENewShopScreen { Main }
	
	public enum EGoalSignal { MakeRequests, CreateItem, EditShop, FreeRoam }
    
	public class ShopAgent : AgentScreen, IEconomyAgent, IScreenSelect<ENewShopScreen>
	{
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
			wallet.onLoseMoney = OnLoseMoney;
			agentInventory.onItemAdd += OnCraft;
			_behaviorParameters = GetComponent<BehaviorParameters>();
		}

		private void OnLoseMoney(float amount)
		{
			if (TrainingConfig.OnSpend)
			{
				var spendPunishment = amount / TrainingConfig.MaxPrice;
				AddReward(-(spendPunishment * TrainingConfig.SellDiscount));
			}
		}

		private void OnEarnMoney(float amount)
		{
			if (TrainingConfig.OnSell)
			{
				var sellReward = amount / TrainingConfig.MaxPrice;
				AddReward(sellReward);
			}
		}

		private void OnCraft(UsableItem usableItem)
		{
			if (TrainingConfig.OnCraft)
			{
				AddReward(TrainingConfig.OnCraftReward);
			}
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
            
            craftingInventory.ResetInventory();
            agentInventory.Reset();
            wallet.Setup(shopInput.requestSystem.system.requestSystem, AgentType.Shop);
            var shopBehaviour = FindObjectOfType<AdventurerShopSubSystem>();
            shopBehaviour.shopCraftingSystem.system.shopSubSubSystem.RemoveShop(this);
            ResetOnItem.bSetupSystems = true;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
	        if (_forcedAction != EShopAgentChoices.None)
	        {
		        var actions = actionsOut.DiscreteActions;
		        actions[0] = (int)_forcedAction;
	        }
        }
        
        readonly Dictionary<EShopAgentChoices, ECraftingResources> _selectRequestMap = new Dictionary<EShopAgentChoices, ECraftingResources>
        {
	        { EShopAgentChoices.SelectWood, ECraftingResources.Wood },
	        { EShopAgentChoices.SelectMetal, ECraftingResources.Metal },
	        { EShopAgentChoices.SelectGem, ECraftingResources.Gem },
	        { EShopAgentChoices.SelectDragonScale, ECraftingResources.DragonScale }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingResources> _increasePriceRequestMap = new Dictionary<EShopAgentChoices, ECraftingResources>
        {
	        { EShopAgentChoices.RequestIncreaseWood, ECraftingResources.Wood },
	        { EShopAgentChoices.RequestIncreaseMetal, ECraftingResources.Metal },
	        { EShopAgentChoices.RequestIncreaseGem, ECraftingResources.Gem },
	        { EShopAgentChoices.RequestIncreaseDragonScale, ECraftingResources.DragonScale }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingResources> _decreasePriceRequestMap = new Dictionary<EShopAgentChoices, ECraftingResources>
        {
	        { EShopAgentChoices.RequestDecreaseWood, ECraftingResources.Wood },
	        { EShopAgentChoices.RequestDecreaseMetal, ECraftingResources.Metal },
	        { EShopAgentChoices.RequestDecreaseGem, ECraftingResources.Gem },
	        { EShopAgentChoices.RequestDecreaseDragonScale, ECraftingResources.DragonScale }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingResources> _removeRequestMap = new Dictionary<EShopAgentChoices, ECraftingResources>
        {
	        { EShopAgentChoices.RequestRemoveWood, ECraftingResources.Wood },
	        { EShopAgentChoices.RequestRemoveMetal, ECraftingResources.Metal },
	        { EShopAgentChoices.RequestRemoveGem, ECraftingResources.Gem },
	        { EShopAgentChoices.RequestRemoveDragonScale, ECraftingResources.DragonScale }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingChoice> _craftCraftMap = new Dictionary<EShopAgentChoices, ECraftingChoice>
        {
	        { EShopAgentChoices.CraftBeginnerSword, ECraftingChoice.BeginnerSword },
	        { EShopAgentChoices.CraftIntermediateSword, ECraftingChoice.IntermediateSword } ,
	        { EShopAgentChoices.CraftAdvancedSword, ECraftingChoice.AdvancedSword },
	        { EShopAgentChoices.CraftEpicSword, ECraftingChoice.EpicSword },
	        { EShopAgentChoices.CraftUltimateSword, ECraftingChoice.UltimateSwordOfPower }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingChoice> _craftIncreaseMap = new Dictionary<EShopAgentChoices, ECraftingChoice>
        {
	        { EShopAgentChoices.IncreasePriceBeginnerSword, ECraftingChoice.BeginnerSword },
	        { EShopAgentChoices.IncreasePriceIntermediateSword, ECraftingChoice.IntermediateSword } ,
	        { EShopAgentChoices.IncreasePriceAdvancedSword, ECraftingChoice.AdvancedSword },
	        { EShopAgentChoices.IncreasePriceEpicSword, ECraftingChoice.EpicSword },
	        { EShopAgentChoices.IncreasePriceUltimateSword, ECraftingChoice.UltimateSwordOfPower }
        };
        
        readonly Dictionary<EShopAgentChoices, ECraftingChoice> _craftDecreaseMap = new Dictionary<EShopAgentChoices, ECraftingChoice>
        {
	        { EShopAgentChoices.DecreasePriceBeginnerSword, ECraftingChoice.BeginnerSword },
	        { EShopAgentChoices.DecreasePriceIntermediateSword, ECraftingChoice.IntermediateSword } ,
	        { EShopAgentChoices.DecreasePriceAdvancedSword, ECraftingChoice.AdvancedSword },
	        { EShopAgentChoices.DecreasePriceEpicSword, ECraftingChoice.EpicSword },
	        { EShopAgentChoices.DecreasePriceUltimateSword, ECraftingChoice.UltimateSwordOfPower }
        };

        public override void OnActionReceived(ActionBuffers actions)
        {
	        var battleAction = (EShopAgentChoices) Mathf.FloorToInt(actions.DiscreteActions[0]);
	        if (battleAction is >= EShopAgentChoices.SelectWood 
	            and <= EShopAgentChoices.SelectDragonScale)
	        {
		        shopInput.requestSystem.system.requestSystem.MakeRequest(_selectRequestMap[battleAction], craftingInventory, wallet);
	        }

	        else if (battleAction is >= EShopAgentChoices.RequestIncreaseWood 
	                 and <= EShopAgentChoices.RequestIncreaseDragonScale)
	        {
		        shopInput.requestSystem.system.requestSystem.ChangePrice(_increasePriceRequestMap[battleAction], craftingInventory, wallet, 1);
	        }
	        
	        else if (battleAction is >= EShopAgentChoices.RequestDecreaseWood 
	                 and <= EShopAgentChoices.RequestDecreaseDragonScale)
	        {
		        shopInput.requestSystem.system.requestSystem.ChangePrice(_decreasePriceRequestMap[battleAction], craftingInventory, wallet, 1);
	        }
	        
	        else if (battleAction is >= EShopAgentChoices.RequestRemoveWood 
	                 and <= EShopAgentChoices.RequestRemoveDragonScale)
	        {
		        shopInput.requestSystem.system.requestSystem.RemoveRequest(_removeRequestMap[battleAction], craftingInventory);
	        }
	        
	        else if (battleAction is >= EShopAgentChoices.CraftBeginnerSword
	                 and <= EShopAgentChoices.CraftUltimateSword)
	        {
		        shopInput.shopCraftingSystem.system.craftingSubSubSystem.MakeCraftRequest(this, _craftCraftMap[battleAction]);
	        }
	        
	        else if (battleAction is >= EShopAgentChoices.IncreasePriceBeginnerSword
	                 and <= EShopAgentChoices.IncreasePriceUltimateSword)
	        {
		        shopInput.shopCraftingSystem.system.shopSubSubSystem.SetCurrentPrice(this, _craftIncreaseMap[battleAction], 1);
	        }
	        
	        else if (battleAction is >= EShopAgentChoices.DecreasePriceBeginnerSword
	                 and <= EShopAgentChoices.DecreasePriceBeginnerSword)
	        {
		        shopInput.shopCraftingSystem.system.shopSubSubSystem.SetCurrentPrice(this, _craftDecreaseMap[battleAction], -1);
	        }
        }

        public void SetAction(EShopAgentChoices choice)
		{
			_bForcedAction = true;
			_forcedAction = choice;
			RequestDecision();
		}

		public void SetAction(int action, bool shop)
		{
			SetAction((EShopAgentChoices) action);
		}

		public List<EShopAgentChoices[]> GetEnabledActions()
		{
			var craftInputs = shopInput.shopCraftingSystem.system.GetEnabledInputs(this);
			
			var requestInputs = shopInput.requestSystem.system.GetEnabledInputs(this);

			return new List<EShopAgentChoices[]> {craftInputs, requestInputs };
		}
		
		private BehaviorParameters _behaviorParameters;

		public IEnumerable<EnabledInput> GetEnabledInput()
		{
			var toReturn = new List<EnabledInput>();
			if (_behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
			{
				var actions = GetEnabledActions();
				foreach (var input in ValuesAsArray)
				{
					var enabled = actions[0].Contains(input) || actions[1].Contains(input);
					var enabledInput = new EnabledInput
					{
						Enabled = enabled,
						Input = (int)input
					};
					toReturn.Add(enabledInput);
				}
			}

			return toReturn;
		}
		
		private static readonly EShopAgentChoices [] ValuesAsArray
			= Enum.GetValues(typeof(EShopAgentChoices)).Cast<EShopAgentChoices>().ToArray();
		
		public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
		{
			var actions = GetEnabledActions();
			foreach (var input in ValuesAsArray)
			{
				var contains = false;
				foreach (var a in actions)
				{
					if (a.Contains(input))
					{
						contains = true;
					}
				}
				actionMask.SetActionEnabled(0, (int) input, contains);
			}
		}

		public int HalfSize => 6;
	}
}
