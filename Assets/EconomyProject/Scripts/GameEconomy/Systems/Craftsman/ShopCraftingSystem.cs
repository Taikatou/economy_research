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
    public enum ECraftingOptions { Craft, SubmitToShop, EditShop }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
	    public override int ObservationSize => 2;
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        private AgentStateSelector<ShopAgent, ECraftingOptions> _agentChoices;

        public CraftLocationMap CraftLocationMap { get; set; }
        
        public ShopLocationMap ShopLocationMap { get; set; }

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

        public void BackButton(ShopAgent agent)
        {
	        CraftingLocationMap.RemoveAgent(agent);
	        CraftLocationMap.RemoveAgent(agent);
	        ShopLocationMap.RemoveAgent(agent);
	        AgentInput.ChangeScreen(agent, EShopScreen.Main);
        }
        
        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
	        switch (input)
	        {
		        case EShopAgentChoices.Back:
			        BackButton(agent);
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
			        SetState(agent, ECraftingOptions.SubmitToShop);
			        break;
		        case EShopAgentChoices.Craft:
			        SetState(agent, ECraftingOptions.Craft);
			        break;
		        case EShopAgentChoices.EditPrice:
			        SetState(agent, ECraftingOptions.EditShop);
			        break;
		        case EShopAgentChoices.IncreasePrice:
			        PriceUpDown(agent, 1);
			        break;
		        case EShopAgentChoices.DecreasePrice:
			        PriceUpDown(agent, -1);
			        break;
	        }
        }

        public void SetState(ShopAgent agent, ECraftingOptions state)
        {
	        _agentChoices.SetState(agent, state);
        }

        public void PriceUpDown(ShopAgent agent, int increment)
        {
	        var index = ShopLocationMap.GetCurrentLocation(agent);
	        var items = shopSubSubSystem.GetShopItems(agent);

	        if (items.Count > index)
	        {
		        var item = items[index];
		        shopSubSubSystem.SetCurrentPrice(agent, item, increment);
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
			        shopSubSubSystem.SubmitToShop(agent, CraftLocationMap.GetCraftingChoice(agent).Item);	
			        break;
		        case ECraftingOptions.EditShop:
			        
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
		        case ECraftingOptions.SubmitToShop:
			        CraftLocationMap.MovePosition(agent, movement);
			        break;
		        case ECraftingOptions.EditShop:
			        ShopLocationMap.MovePosition(agent, movement);
			        break;
	        }
        }

        public int GetIndexInShopList(ShopAgent shopAgent, UsableItem item)
		{
			var items = shopSubSubSystem.GetShopItems(shopAgent);
			for (var i = 0; i < items.Count; i++)
			{
				if (item == items[i])
				{
					return i;
				}
			}

			Debug.LogError("Item not in shop : " + item.itemDetails.itemName);
			return -1;
		}

		public void FixedUpdate()
        {
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
				EShopAgentChoices.Craft,
				EShopAgentChoices.EditPrice,
				EShopAgentChoices.IncreasePrice,
				EShopAgentChoices.DecreasePrice
			};
			var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
			return outputs;
		}
    }
}
