using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Interfaces;
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

        public ObsData[] GetSenses()
        {
            return new ObsData [SenseCount]
            {
                new ObsData { data=Progress, name="CraftingProgress"}
            };
        }

        public static ObsData[] GetTemplateSenses()
        {
	        return new ObsData [SenseCount]
	        {
		        new ObsData { data=0, name="CraftingProgress"}
	        };
        }

        public const int SenseCount = 1;
    }
    public enum ECraftingOptions { Craft, SubmitToShop, EditShop }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
	    public static int SensorCount = 2;
	    public static int ObservationSize => SensorCount + AgentShopSubSystem.SensorCount + CraftingSubSystem.SenseCount;
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        private AgentStateSelector<ShopAgent, ECraftingOptions> _agentChoices;

        public CraftLocationMap CraftLocationMap { get; set; }
        
        public ShopLocationMap ShopLocationMap { get; set; }
        
        public CraftingRequestLocationMap CraftingLocationMap { get; set; }
        
        public void Setup()
        {
	        _agentChoices.Setup();
        }

        public ECraftingOptions GetState(ShopAgent agent)
        {
	        return _agentChoices.GetState(agent);
        }

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

		public int GetScrollLocation(ShopAgent agent)
		{
			var index = 0;
			var state = _agentChoices.GetState(agent);
			switch (state)
			{
				case ECraftingOptions.Craft:
					index = CraftingLocationMap.GetCurrentLocation(agent);
					break;
				case ECraftingOptions.SubmitToShop:
					index = CraftLocationMap.GetCurrentLocation(agent);
					break;
				case ECraftingOptions.EditShop:
					index = ShopLocationMap.GetCurrentLocation(agent);
					break;
			}
			return index;
		}

        public override ObsData[] GetObservations(ShopAgent agent)
        {
	        var state = _agentChoices.GetState(agent);
	        var shopL = GetScrollLocation(agent);
			var outputs = new List<ObsData>
			{
				new ObsData
				{
					data=(float)state, 
					name="craftingState"
				},
				new ObsData
				{
					data=shopL,
					name="shopLocation"
				},
			};
			var sensesA = shopSubSubSystem.GetItemSenses(agent);
			outputs.AddRange(sensesA);
            var sensesB = craftingSubSubSystem.GetObservations(agent);
            outputs.AddRange(sensesB);
            return outputs.ToArray();
        }

        public void SetPrice(ShopAgent shopAgent, UsableItem item, int increment)
		{
			var index = GetIndexInShopList(shopAgent, item);
			if (index == -1)
			{
				return;
			}

			var option = increment < 0 ? EShopAgentChoices.DecreasePrice : EShopAgentChoices.IncreasePrice;
			SetChoice(shopAgent, option);
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
	        var items = shopSubSubSystem.GetShopUsableItems(agent);

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
			        var shopItem = CraftLocationMap.GetCraftingChoice(agent);
			        if (shopItem.HasValue)
			        {
				        shopSubSubSystem.SubmitToShop(agent, shopItem.Value.Item);	   
			        }
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
			var items = shopSubSubSystem.GetShopUsableItems(shopAgent);
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
			var state = GetState(agent);
			var inputChoices = new List<EShopAgentChoices>
			{
				EShopAgentChoices.Up,
				EShopAgentChoices.Down,
				EShopAgentChoices.Select,
				EShopAgentChoices.Back
			};
			switch (state)
			{
				case ECraftingOptions.Craft:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.SubmitToShop,
						EShopAgentChoices.EditPrice
					});
					break;
				case ECraftingOptions.SubmitToShop:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.EditPrice,
						EShopAgentChoices.Craft
					});
					break;
				case ECraftingOptions.EditShop:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.IncreasePrice,
						EShopAgentChoices.DecreasePrice,
						EShopAgentChoices.Craft
					});
					break;
			}
			var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
			return outputs;
		}
    }
}
