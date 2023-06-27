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
    public enum EShopRequestStates { MakeRequest, ChangePrice }

    [Serializable]
    public class RequestShopSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>, ISetup
    {
        public RequestSystem requestSystem;
        public static int ObservationSize => 13;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public ShopRequestLocationMap MakeRequestGetLocation { get; set; }
        
        public ShopRequestLocationMap ChangePriceGetLocation { get; set; }
        
        private AgentStateSelector<ShopAgent, EShopRequestStates> _agentStateSelector;

        public EShopRequestStates GetState(ShopAgent agent)
        {
            return _agentStateSelector.GetState(agent);
        }

        public void Start()
        {
            _agentStateSelector =
                new AgentStateSelector<ShopAgent, EShopRequestStates>((EShopRequestStates.MakeRequest));
        }

        public void Setup()
        {
            _agentStateSelector.Setup();
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
            var state = _agentStateSelector.GetState(agent);
            var item = MakeRequestGetLocation.GetItem(agent);
            var changePrice = ChangePriceGetLocation.GetItem(agent);
            
            
            var outputSenses = new List<ObsData>
            {
                new CategoricalObsData<EShopRequestStates>(state) { Name = "AgentState" },
                GetItemObs(item, "resource selection"),
                GetItemObs(changePrice, "resource selection"),
                new SingleObsData(){ data=ChangePriceGetLocation.GetLimit(agent), Name = "currentRequestLimit"}
            };
            return outputSenses.ToArray();
        }
        
        public void GetRequestObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var inventory = agent.craftingInventory;
            var items = requestSystem.GetAllCraftingRequests(inventory);

            var outputs = new List<float>();
            foreach (var request in items)
            {
                var r = new float[Enum.GetNames(typeof(ECraftingResources)).Length];
                r[(int)request.Resource] = 1;
                outputs.AddRange(r);
                outputs.Add(request.Price);
                outputs.Add(request.Number);
            }

            if (bufferSensorComponent != null)
            {
                bufferSensorComponent.AppendObservation(outputs.ToArray());
            }
        }

        public LocationSelect<ShopAgent> GetLocationSelect(ShopAgent agent)
        {
            var state = _agentStateSelector.GetState(agent);
            return state == EShopRequestStates.MakeRequest ? MakeRequestGetLocation : ChangePriceGetLocation;
        }

        public void Select(ShopAgent agent)
        {
            var state = _agentStateSelector.GetState(agent);
            if(state == EShopRequestStates.MakeRequest)
            {
                var resource = MakeRequestGetLocation.GetItem(agent);
                if (resource.HasValue)
                {
                    requestSystem.MakeRequest(resource.Value, agent.craftingInventory, agent.wallet);
                }
            }
        }

        public void RemoveRequest(ShopAgent agent)
        {
            var state = _agentStateSelector.GetState(agent);
            if (state == EShopRequestStates.ChangePrice)
            {
                var resource = ChangePriceGetLocation.GetItem(agent);
                if (resource.HasValue)
                {
                    requestSystem.RemoveRequest(resource.Value, agent.craftingInventory);   
                }
            }
        }

        public RequestSystemLocationSelect requestLocationSelect;

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            switch (input)
            {
                case EShopAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EShopScreen.Main);
                    break;
                case EShopAgentChoices.Down:
                    GetLocationSelect(agent).MovePosition(agent, -1);
                    break;
                case EShopAgentChoices.Up:
                    GetLocationSelect(agent).MovePosition(agent, 1);
                    break;
                case EShopAgentChoices.Select:
                    Select(agent);
                    break;
                case EShopAgentChoices.IncrementMode:
                    requestLocationSelect.MovePosition(agent, 1);
                    var subSystem = requestLocationSelect.GetShopOption(agent);
                    _agentStateSelector.SetState(agent, subSystem);
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
            var state = _agentStateSelector.GetState(agent);
            if (state == EShopRequestStates.ChangePrice)
            {
                var resource = ChangePriceGetLocation.GetItem(agent);
                if (resource.HasValue)
                {
                    requestSystem.ChangePrice(resource.Value, agent.craftingInventory, agent.wallet, value);   
                }
            }
        }

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
        {
            var inputChoices = new List<EShopAgentChoices>
            {
                EShopAgentChoices.Up,
                EShopAgentChoices.Down,
                EShopAgentChoices.Back
            };
            var state = _agentStateSelector.GetState(agent);
            if (state == EShopRequestStates.ChangePrice)
            {
                inputChoices.AddRange(new []
                {
                    EShopAgentChoices.IncreasePrice,
                    EShopAgentChoices.DecreasePrice,
                    EShopAgentChoices.IncrementMode
                });
            }
            else
            {
                inputChoices.AddRange(new[]
                {
                    EShopAgentChoices.IncrementMode,
                    EShopAgentChoices.Select
                });
            }
            
            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
            return outputs;
        }
    }
}
