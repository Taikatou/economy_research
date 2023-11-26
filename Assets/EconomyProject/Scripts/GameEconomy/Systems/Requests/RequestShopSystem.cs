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
        public static int ObservationSize => 4;
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
            var item = MakeRequestGetLocation.GetItem(agent);

            var outputSenses = new List<ObsData>
            {
                GetItemObs(item, "resource selection"),
            };
            GetRequestObservations(agent, bufferSensorComponent[0]);
            return outputSenses.ToArray();
        }
        
        public void GetRequestObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var items = requestSystem.GetAllCraftingRequests(agent.craftingInventory);

            foreach (var request in items.Values)
            {
                var outputs = new List<float>();
                var r = new float[4];
                if (request.Resource > ECraftingResources.Nothing)
                {
                    var index = (int)request.Resource - 1;
                    if(index < r.Length)
                        r[index] = 1;
                    else
                        Debug.Log(request.Resource);
                }

                outputs.AddRange(r);
                outputs.Add(request.Price);
                outputs.Add(request.Number);
                bufferSensorComponent.AppendObservation(outputs.ToArray());
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
        
        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            switch (input)
            {
                case EShopAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EShopScreen.Main);
                    break;
                case EShopAgentChoices.Down:
                    MakeRequestGetLocation.MovePosition(agent, -1);
                    break;
                case EShopAgentChoices.Up:
                    MakeRequestGetLocation.MovePosition(agent, 1);
                    break;
                case EShopAgentChoices.Select:
                    Select(agent);
                    break;
                case EShopAgentChoices.IncreasePrice:
                    ChangePrice(agent, 1);
                    break;
                case EShopAgentChoices.DecreasePrice:
                    ChangePrice(agent, -1);
                    break;
            case EShopAgentChoices.RemoveRequest:
                    RemoveRequest(agent);
                    break;
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

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent, int branch)
        {
            var inputChoices = new List<EShopAgentChoices>
            {
                EShopAgentChoices.Back,
                EShopAgentChoices.Select
            };
            var resource = MakeRequestGetLocation.GetItem(agent);
            if (resource != ECraftingResources.Wood)
            {
                inputChoices.Add(EShopAgentChoices.Down);
            }
            if (resource != ECraftingResources.DragonScale)
            {
                inputChoices.Add(EShopAgentChoices.Up);
            }
            
            var requests = requestSystem.GetAllCraftingRequests(agent.craftingInventory);
            
            if (resource.HasValue)
            {
                var foundRequest = false;
                foreach (var req in requests.Values)
                {
                    if (req.Resource == resource.Value)
                    {
                        foundRequest = true;
                        break;
                    }
                }

                if (foundRequest)
                {
                    inputChoices.AddRange( new []
                    {
                        EShopAgentChoices.IncreasePrice,
                        EShopAgentChoices.DecreasePrice,
                        EShopAgentChoices.RemoveRequest
                    });   
                }   
            }

            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices, branch);
            return outputs;
        }

        public void Setup()
        {
            
        }
    }
}
