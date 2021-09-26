using System;
using System.Collections.Generic;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using Debug = UnityEngine.Debug;

namespace EconomyProject.Scripts.GameEconomy.Systems.Craftsman
{
    [System.Serializable]
    public struct CraftingMap
    {
        public ECraftingChoice choice;
        public CraftingRequirements resource;
    }
    
    public class CraftingRequest
    {
        public CraftingRequirements CraftingRequirements;
        public float CraftingTime;

        public float Progress => CraftingTime / CraftingRequirements.timeToCreation;

        public bool Complete => CraftingTime >= CraftingRequirements.timeToCreation;

        public float[] GetSenses()
        {
            return new float [SenseCount]
            {
                Progress
            };
        }

        public const int SenseCount = 1;
    }

    public enum ECraftingChoice { BeginnerSword, IntermediateSword, AdvancedSword, EpicSword, MasterSword, UltimateSwordOfPower }
    public enum ECraftingOptions { Craft, SubmitToShop }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
	    public override int ObservationSize => 2;
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        private AgentStateSelector<ShopAgent, ECraftingOptions> _agentChoices;

        public ShopLocationMap shopLocationMap { get; set; }

        public ECraftingOptions GetState(ShopAgent agent)
        {
	        return _agentChoices.GetState(agent);
        }
        
        public CraftingRequestLocationMap CraftingLocationMap { get; set; }

        public void Start()
		{
			if (craftingSubSubSystem == null)
			{
				craftingSubSubSystem = new CraftingSubSystem();
			}

			_agentChoices = new AgentStateSelector<ShopAgent, ECraftingOptions>(ECraftingOptions.Craft);
		}

		public override bool CanMove(ShopAgent agent)
        {
            return !craftingSubSubSystem.HasRequest(agent);
        }

        public override float[] GetObservations(ShopAgent agent)
        {
			var outputs = new float [1 + AgentShopSubSystem.SenseCount + CraftingSubSystem.SenseCount];
			outputs[0] = 0; //(float) GetInputMode(agent);
            var sensesA = shopSubSubSystem.GetSenses(agent);
            sensesA.CopyTo(outputs, 1);
            var sensesB = craftingSubSubSystem.GetObservations(agent);
            sensesB.CopyTo(outputs, 1 + sensesA.Length);
            return outputs;
        }

        public void SetPrice(ShopAgent shopAgent, UsableItem item, int increment)
		{
			var index = GetIndexInShopList(shopAgent, item);
			if (index == -1)
			{
				return;
			}

			if(increment < 0)
			{
				SetChoice(shopAgent, EShopAgentChoices.DecreasePrice);
			}
			else
			{
				SetChoice(shopAgent, EShopAgentChoices.IncreasePrice);
			}
		}
        
        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
	        switch (input)
	        {
		        case EShopAgentChoices.Back:
			        AgentInput.ChangeScreen(agent, EShopScreen.Main);
			        break;
		        case EShopAgentChoices.Select:
			        Select(agent);
			        break;
		        case EShopAgentChoices.Down:
			        UpDown(agent, -1);
			        break;
		        case EShopAgentChoices.Up:
					UpDown(agent, 1);
			        break;
		        case EShopAgentChoices.SubmitToShop:
			        _agentChoices.SetState(agent, ECraftingOptions.SubmitToShop);
			        break;
		        case EShopAgentChoices.Craft:
			        _agentChoices.SetState(agent, ECraftingOptions.Craft);
			        break;
	        }
        }

        public void Select(ShopAgent agent)
        {
	        var state = _agentChoices.GetState(agent);
	        switch (state)
	        {
		        case ECraftingOptions.Craft:
			        var resource = CraftingLocationMap.GetCraftingChoice(agent);
			        craftingSubSubSystem.MakeRequest(agent, resource);
			        break;
		        case ECraftingOptions.SubmitToShop:
			        shopSubSubSystem.SubmitToShop(agent, shopLocationMap.GetCraftingChoice(agent).Item);	
			        break;
	        }
        }
        
        public void UpDown(ShopAgent agent, int movement)
        {
	        var state = _agentChoices.GetState(agent);
	        switch (state)
	        {
		        case ECraftingOptions.Craft:
					CraftingLocationMap.MovePosition(agent, movement);
			        break;
	        }
        }

        public int GetIndexInShopList(ShopAgent shopAgent, UsableItem item)
		{
			var items = shopSubSubSystem.GetShopItems(shopAgent);
			for (int i = 0; i < items.Count; i++)
			{
				if (item == items[i])
				{
					return i;
				}
			}

			Debug.LogError("Item not in shop : " + item.itemDetails.itemName);
			return -1;
		}

		public void Update()
        {
			RequestDecisions();
			craftingSubSubSystem.Update();
		}
		
		public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
		{
			var inputChoices = new []
			{
				EShopAgentChoices.Up,
				EShopAgentChoices.Down,
				EShopAgentChoices.Select,
				EShopAgentChoices.Back,
				EShopAgentChoices.SubmitToShop,
				EShopAgentChoices.Craft
			};
			var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
			return outputs;
		}
    }
}
