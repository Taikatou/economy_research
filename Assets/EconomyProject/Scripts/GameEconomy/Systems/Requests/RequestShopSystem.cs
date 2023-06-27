using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Requests.ShopLocationMaps;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{

    [Serializable]
    public class RequestShopSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
        public RequestSystem requestSystem;
        public static int ObservationSize => 4;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public ShopRequestLocationMap MakeRequestGetLocation { get; set; }

        public void Setup()
        {
        }

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

        public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var item = MakeRequestGetLocation.GetItem(agent);

            var outputSenses = new List<ObsData>
            {
                GetItemObs(item, "resource selection"),
            };
            GetRequestObservations(agent, bufferSensorComponent);
            return outputSenses.ToArray();
        }
        
        public void GetRequestObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var inventory = agent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);

            
            foreach (var request in items)
            {
                var outputs = new List<float>();
                var r = new float[Enum.GetNames(typeof(ECraftingResources)).Length];
                r[(int)request.Resource] = 1;
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

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
        {
            var inputChoices = new List<EShopAgentChoices>
            {
                EShopAgentChoices.Up,
                EShopAgentChoices.Down,
                EShopAgentChoices.Back,
                EShopAgentChoices.IncreasePrice,
                EShopAgentChoices.DecreasePrice,
                EShopAgentChoices.Select
            };

            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
            return outputs;
        }
    }
}
