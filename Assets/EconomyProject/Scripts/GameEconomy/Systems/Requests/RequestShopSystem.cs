using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{

    [Serializable]
    public class RequestShopSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
        public RequestSystem requestSystem;
        public static int ObservationSize => 0;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public ShopRequestLocationMap MakeRequestGetLocation { get; set; }

        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        private ObsData GetItemObs(ECraftingResources? resource, string name)
        {
            return resource.HasValue
                ? new CategoricalObsData<ECraftingResources>(resource.Value){ Name = name }
                : new CategoricalObsData<ECraftingResources>(ECraftingResources.Nothing){ Name = name };
        }

        public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            GetRequestObservations(agent, bufferSensorComponent[0]);
            return new ObsData[] { };
        }
        
        public void GetRequestObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var items = requestSystem.GetAllCraftingRequests(agent.craftingInventory);

            foreach (var request in items.Values)
            {
                if (request.Resource > ECraftingResources.Nothing)
                {
                    var index = (int)request.Resource - 1;
                    var r = new float[6];
                    r[index] = 1;
                    r[4] = request.Price;
                    r[5] = request.Number;
                    
                    bufferSensorComponent.AppendObservation(r);
                }
            }
        }

        public void Select(ShopAgent agent)
        {
            var resource = MakeRequestGetLocation.GetItem(agent);
            if (resource.HasValue)
            {
                requestSystem.MakeRequest(resource.Value, agent.craftingInventory, agent.wallet);
            }
        }

        public void RemoveRequest(ShopAgent agent)
        {
            var resource = MakeRequestGetLocation.GetItem(agent);
            if (resource.HasValue)
            {
                requestSystem.RemoveRequest(resource.Value, agent.craftingInventory);   
            }
        }

        public void ChangePrice(ShopAgent agent, int value)
        {
            var resource = MakeRequestGetLocation.GetItem(agent);
            if (resource.HasValue)
            {
                requestSystem.ChangePrice(resource.Value, agent.craftingInventory, agent.wallet, value);   
            }
        }
        
        private readonly ECraftingResources [] _resources = Enum.GetValues(typeof(ECraftingResources)).Cast<ECraftingResources>().ToArray();

        private readonly Dictionary<ECraftingResources, EShopAgentChoices> _craftMap =
            new()
            {
                { ECraftingResources.Nothing, EShopAgentChoices.None},
                { ECraftingResources.Wood, EShopAgentChoices.SelectWood },
                { ECraftingResources.Metal, EShopAgentChoices.SelectMetal },
                { ECraftingResources.Gem, EShopAgentChoices.SelectGem },
                { ECraftingResources.DragonScale, EShopAgentChoices.SelectDragonScale }
            };
        
        private readonly Dictionary<ECraftingResources, List<EShopAgentChoices>> _increasePriceMap =
            new()
            {
                { ECraftingResources.Nothing, new List<EShopAgentChoices>()},
                { ECraftingResources.Wood, new List<EShopAgentChoices> {EShopAgentChoices.RequestIncreaseWood, EShopAgentChoices.RequestDecreaseWood, EShopAgentChoices.RequestRemoveWood} },
                { ECraftingResources.Metal, new List<EShopAgentChoices> {EShopAgentChoices.RequestIncreaseMetal, EShopAgentChoices.RequestDecreaseMetal, EShopAgentChoices.RequestRemoveMetal} },
                { ECraftingResources.Gem, new List<EShopAgentChoices> {EShopAgentChoices.RequestIncreaseGem, EShopAgentChoices.RequestDecreaseGem, EShopAgentChoices.RequestRemoveGem} },
                { ECraftingResources.DragonScale, new List<EShopAgentChoices> {EShopAgentChoices.RequestIncreaseDragonScale, EShopAgentChoices.RequestDecreaseDragonScale, EShopAgentChoices.RequestRemoveDragonScale} }
            };
        public override EShopAgentChoices[] GetEnabledInputs(ShopAgent agent)
        {
            var options = new HashSet<EShopAgentChoices>();
            var requests = requestSystem.GetAllCraftingRequests(agent.craftingInventory);
            foreach (var r in _resources)
            {
                var canSelect = requestSystem.CanMakeRequest(agent.craftingInventory, r);
                if (canSelect)
                {
                    options.Add(_craftMap[r]);
                }
            }
            
            var craftSet = new HashSet<ECraftingResources>();
            
            foreach (var req in requests.Values)
            {
                if (craftSet.Add(req.Resource)) // returns false if already exists
                {
                    foreach (var o in _increasePriceMap[req.Resource])
                    {
                        options.Add(o);
                    }
                }
            }
            
            return options.ToArray();
        }

        public void Setup()
        {
            
        }
    }
}
