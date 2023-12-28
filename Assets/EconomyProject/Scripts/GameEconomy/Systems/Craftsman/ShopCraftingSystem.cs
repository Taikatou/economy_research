using System;
using System.Collections.Generic;
using System.Linq;
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
        public float CraftingTime { get; set; }

        public float Progress => CraftingTime / CraftingRequirements.TimeToCreation;

        public bool Complete => CraftingTime >= CraftingRequirements.TimeToCreation;

        public ObsData GetCraftingProgressionObservation()
        {
	        return new SingleObsData { data = Progress, Name = "CraftingProgress" };
        }

        public const int SenseCount = 2;
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
			var outputs = new List<ObsData>(craftingSubSubSystem.GetObservations(agent, null));
			var resources = Enum.GetValues(typeof(ECraftingResources)).Cast<ECraftingResources>();
			foreach (var resource in resources)
			{
				outputs.Add(
					new SingleObsData { data = agent.craftingInventory.GetResourceNumber(resource), Name = "resource" });
			}
			
			shopSubSubSystem.UpdateShopSenses(agent, bufferSensorComponent[1]);
			shopSubSubSystem.GetItemSenses(bufferSensorComponent[0], agent);
			return outputs.ToArray();
        }

        public void BackButton(ShopAgent agent)
        {
	        CraftingLocationMap.RemoveAgent(agent);
	        SubmitToShopLocationMap.RemoveAgent(agent);
	        ShopLocationMap.RemoveAgent(agent);
	        AgentInput.ChangeScreen(agent, EShopScreen.Main);
        }

        public void PriceUpDown(ShopAgent agent, int increment)
        {
	        throw new NotImplementedException();
        }

        public void Select(ShopAgent agent)
        {
	        var resource = CraftingLocationMap.GetCraftingChoice(agent);
	        craftingSubSubSystem.MakeCraftRequest(agent, resource);
        }

        public int GetIndexInShopList(ShopAgent shopAgent, UsableItem item)
        {
	        throw new NotImplementedException();
		}

		public void FixedUpdate()
        {
	        craftingSubSubSystem.Update();
		}
		
		private readonly ECraftingChoice [] _craftingChoices = Enum.GetValues(typeof(ECraftingChoice)).Cast<ECraftingChoice>().ToArray();

		private readonly Dictionary<ECraftingChoice, List<EShopAgentChoices>> _values =
			new Dictionary<ECraftingChoice, List<EShopAgentChoices>>()
			{
				{ ECraftingChoice.BeginnerSword,  new List<EShopAgentChoices>
					{
						EShopAgentChoices.IncreasePriceBeginnerSword,
						EShopAgentChoices.DecreasePriceBeginnerSword
					}
				},
				{ ECraftingChoice.AdvancedSword,  new List<EShopAgentChoices>
				{
					EShopAgentChoices.IncreasePriceAdvancedSword,
					EShopAgentChoices.DecreasePriceAdvancedSword
				}},
				{ ECraftingChoice.IntermediateSword,  new List<EShopAgentChoices>
				{
					EShopAgentChoices.IncreasePriceIntermediateSword,
					EShopAgentChoices.DecreasePriceIntermediateSword
				}},
				{ ECraftingChoice.EpicSword,  new List<EShopAgentChoices>
				{
					EShopAgentChoices.IncreasePriceEpicSword,
					EShopAgentChoices.DecreasePriceEpicSword
				}},
				{ ECraftingChoice.MasterSword,  new List<EShopAgentChoices>
				{
					EShopAgentChoices.IncreasePriceMasterSword,
					EShopAgentChoices.DecreasePriceMasterSword
				}},
				{ ECraftingChoice.UltimateSwordOfPower,  new List<EShopAgentChoices>
				{
					EShopAgentChoices.IncreasePriceUltimateSword,
					EShopAgentChoices.DecreasePriceUltimateSword
				}
			}
			};

		private readonly Dictionary<ECraftingChoice, EShopAgentChoices> craftMap =
			new()
			{
				{ ECraftingChoice.BeginnerSword, EShopAgentChoices.CraftBeginnerSword },
				{ ECraftingChoice.IntermediateSword, EShopAgentChoices.CraftIntermediateSword },
				{ ECraftingChoice.AdvancedSword, EShopAgentChoices.CraftAdvancedSword },
				{ ECraftingChoice.EpicSword, EShopAgentChoices.CraftEpicSword },
				{ ECraftingChoice.UltimateSwordOfPower, EShopAgentChoices.CraftUltimateSword },
				{ ECraftingChoice.MasterSword, EShopAgentChoices.CraftMasterSword }
			};
		public override EShopAgentChoices[] GetEnabledInputs(ShopAgent agent)
		{
			var inputChoices = new HashSet<EShopAgentChoices>();

			foreach (var choice in _craftingChoices)
			{
				if (craftingSubSubSystem.CanCraft(agent, choice))
				{
					inputChoices.Add(craftMap[choice]);
				}
			}
			
			
			var items = shopSubSubSystem.GetShopUsableItems(agent);
			foreach (var itemPair in items)
			{
				foreach (var o in _values[itemPair.Key])
				{
					inputChoices.Add(o);
				}
			}
			var found = false;
			
			return inputChoices.ToArray();
		}

		public void Setup()
		{
			
		}
    }
}
