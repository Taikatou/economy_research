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

    [Serializable]
    public class ShopCraftingSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
	    public override EShopScreen ActionChoice => EShopScreen.Craft;

	    public CraftingSubSystem craftingSubSubSystem;

        public AgentShopSubSystem shopSubSubSystem;

        public CraftLocationMap SubmitToShopLocationMap { get; set; }
        
        public ShopLocationMap ShopLocationMap { get; set; }
        
        public CraftingRequestLocationMap CraftingLocationMap { get; set; }

        public void Start()
        {
	        if (craftingSubSubSystem == null)
	        {
		        craftingSubSubSystem = new CraftingSubSystem();
	        }

	        craftingSubSubSystem.shopSubSubSystem = shopSubSubSystem;
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

		public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
	        var outputs = new List<ObsData>
			{
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
			
			shopSubSubSystem.UpdateShopSenses(agent, bufferSensorComponent[1]);
			shopSubSubSystem.GetItemSenses(bufferSensorComponent[0], agent);
			
			outputs.AddRange(craftingSubSubSystem.GetObservations(agent, null));
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
			        CraftingLocationMap.MovePosition(agent, -1);
			        break;
		        case EShopAgentChoices.Up:
			        CraftingLocationMap.MovePosition(agent, 1);
			        break;
		        case EShopAgentChoices.IncreasePrice:
			        PriceUpDown(agent, 1);
			        break;
		        case EShopAgentChoices.DecreasePrice:
			        PriceUpDown(agent, -1);
			        break;
	        }
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
	        var resource = CraftingLocationMap.GetCraftingChoice(agent);
	        craftingSubSubSystem.MakeRequest(agent, resource);
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
			var inputChoices = new List<EShopAgentChoices>
			{
				EShopAgentChoices.Select,
				EShopAgentChoices.Back
			};

			if (CraftingLocationMap.GetCurrentLocation(agent) < CraftingLocationMap.GetLimit(agent) - 1)
			{
				inputChoices.Add(EShopAgentChoices.Up);
			}
			if (CraftingLocationMap.GetCurrentLocation(agent) > 0)
			{
				inputChoices.Add(EShopAgentChoices.Down);
			}

			var choice = CraftingLocationMap.GetCraftingChoice(agent);
			var items = shopSubSubSystem.GetShopUsableItems(agent);
			var found = false;
			foreach (var i in items)
			{
				if (i.craftChoice == choice)
				{
					found = true;
					break;
				}
			}

			if(found)
			{
				inputChoices.AddRange(
					new [] {
						EShopAgentChoices.IncreasePrice,
						EShopAgentChoices.DecreasePrice
					});
			}
			
			
			var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
			return outputs;
		}

		public void Setup()
		{
			
		}
    }
}
