using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.Interfaces;
using Inventory;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using EconomyProject.Scripts.UI.Craftsman.Crafting;
using EconomyProject.Scripts.UI.Inventory;
using Unity.MLAgents.Sensors;
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

        public ObsData GetCraftingProgressionObservation()
        {
	        return new SingleObsData { data = Progress, Name = "CraftingProgress" };
        }

        public const int SenseCount = 1;
    }
    public enum ECraftingOptions { Craft, SubmitToShop, EditShop }

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        private AgentStateSelector<ShopAgent, ECraftingOptions> _agentChoices;

        public CraftLocationMap SubmitToShopLocationMap { get; set; }
        
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

		public ObsData GetECraftingChoiceObs(ShopItem? value)
		{
			return value.HasValue
				? new CategoricalObsData<ECraftingChoice>(value.Value.Item.craftChoice)
				: new CategoricalObsData<ECraftingChoice>();
		}

		public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
	        var outputs = new List<ObsData>
			{
				new CategoricalObsData<ECraftingOptions>(_agentChoices.GetState(agent))
				{
					Name="craftingState"
				},
				new SingleObsData
				{
					data=CraftingLocationMap.GetLimit(agent),
					Name="shopLocation"
				},
				new SingleObsData
				{
					data=CraftingLocationMap.GetCurrentLocation(agent)
				},
				new CategoricalObsData<ECraftingChoice> (CraftingLocationMap.GetCraftingChoice(agent)),
				new SingleObsData
				{
					data=SubmitToShopLocationMap.GetCurrentLocation(agent)
				},
				GetECraftingChoiceObs(SubmitToShopLocationMap.GetShopItemChoice(agent)),
				new SingleObsData
				{
					data=ShopLocationMap.GetCurrentLocation(agent)
				},
				new SingleObsData
				{
					data=ShopLocationMap.GetLimit(agent)
				}
			};
			
			var shopItems = craftingSubSubSystem.craftingRequirement;
			foreach (var item in shopItems)
			{
				var craftInfo = new CraftingInfo(item, agent.craftingInventory);
				foreach (var requirement in craftInfo.craftingMap.resource.resourcesRequirements)
				{
					outputs.AddRange(new ObsData [] {
						new SingleObsData { data = craftInfo.craftingInventory.GetResourceNumber(requirement.type), Name = "resource" },
						new CategoricalObsData<ECraftingResources>(requirement.type)}
					);
				}
			}
			
			shopSubSubSystem.GetItemSenses(agent, bufferSensorComponent);
			outputs.AddRange(craftingSubSubSystem.GetObservations(agent, bufferSensorComponent));
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
	        SubmitToShopLocationMap.RemoveAgent(agent);
	        ShopLocationMap.RemoveAgent(agent);
	        AgentInput.ChangeScreen(agent, EShopScreen.Main);
        }

        public CraftingSystemLocationSelect systemLocationSelect;
        
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
		        case EShopAgentChoices.IncrementMode:
			        systemLocationSelect.MovePosition(agent, 1);
			        var choice = systemLocationSelect.GetCraftingOption(agent);
			        SetState(agent, choice);
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
			        var shopItem = SubmitToShopLocationMap.GetShopItemChoice(agent);
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
			        SubmitToShopLocationMap.MovePosition(agent, movement);
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
			bool GetDownStateFromAgent(ShopAgent agent, LocationSelect<ShopAgent> locationSelect)
			{
				return locationSelect.GetCurrentLocation(agent) > 0;
			}
			bool GetUpStateFromAgent(ShopAgent agent, LocationSelect<ShopAgent> locationSelect)
			{
				return locationSelect.GetCurrentLocation(agent) < locationSelect.GetLimit(agent) - 1;
			}
			
			var state = GetState(agent);

			bool up = false, down = false, select = false;
			var inputChoices = new List<EShopAgentChoices>
			{
				EShopAgentChoices.Back
			};
			switch (state)
			{
				case ECraftingOptions.Craft:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.IncrementMode,
					});
					var resource = CraftingLocationMap.GetCraftingChoice(agent);
					select = craftingSubSubSystem.CanCraft(agent, resource);
					up = GetUpStateFromAgent(agent, CraftingLocationMap);
					down = GetDownStateFromAgent(agent, CraftingLocationMap);
					break;
				case ECraftingOptions.SubmitToShop:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.IncrementMode,
				//		EShopAgentChoices.Craft
					});
					var shopItem = SubmitToShopLocationMap.GetShopItemChoice(agent);
					select = shopItem.HasValue;
					up = GetUpStateFromAgent(agent, SubmitToShopLocationMap);
					down = GetDownStateFromAgent(agent, SubmitToShopLocationMap);
					break;
				case ECraftingOptions.EditShop:
					inputChoices.AddRange(new []
					{
						EShopAgentChoices.IncreasePrice,
						EShopAgentChoices.DecreasePrice,
			//			EShopAgentChoices.Craft,
						EShopAgentChoices.IncrementMode
					});
					up = GetUpStateFromAgent(agent, ShopLocationMap);
					down = GetDownStateFromAgent(agent, ShopLocationMap);
					break;
			}

			if (up)
			{
				inputChoices.Add(EShopAgentChoices.Up);
			}
			if (down)
			{
				inputChoices.Add(EShopAgentChoices.Down);
			}
			if (select)
			{
				inputChoices.Add(EShopAgentChoices.Select);
			}
			var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
			return outputs;
		}
    }
}
