using System;
using System.Collections.Generic;
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
        public static int ObservationSize => CraftingResourceRequest.SensorCount + SensorCount;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public static int SensorCount = 18;

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

        public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent bufferSensorComponent)
        {
            var state = _agentStateSelector.GetState(agent);
            var outputSenses = new List<ObsData>
            {
                new CategoricalObsData<EShopRequestStates>(state)
                {
                    Name = "AgentState"
                }
            };
            var requestSense = requestSystem.GetObservations(agent, bufferSensorComponent);
            outputSenses.AddRange(requestSense);
            return outputSenses.ToArray();
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
