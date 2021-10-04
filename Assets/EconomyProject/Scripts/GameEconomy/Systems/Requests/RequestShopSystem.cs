using System;
using EconomyProject.Scripts.MLAgents.Craftsman.Requirements;
using EconomyProject.Scripts.MLAgents.Shop;

namespace EconomyProject.Scripts.GameEconomy.Systems.Requests
{
    public enum EShopRequestStates { MakeRequest=EShopAgentChoices.MakeRequest, ChangePrice=EShopAgentChoices.ChangeRequest }

    [Serializable]
    public class RequestShopSystem : StateEconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public RequestSystem requestSystem;
        public static int ObservationSize => CraftingResourceRequest.SensorCount + 1;
        public override EShopScreen ActionChoice => EShopScreen.Request;

        public AdvancedLocationSelect<ShopAgent, CraftingResources, EShopRequestStates> GetLocation { get; set; }
        
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

        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override float[] GetObservations(ShopAgent agent)
        {
            var outputSenses = new float[CraftingResourceRequest.SensorCount + 1];
            outputSenses[0] = 0; //(float) GetInputMode(agent);
            var requestSense = requestSystem.GetObservations(agent);
            requestSense.CopyTo(outputSenses, 1);
            return outputSenses;
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            switch (input)
            {
                case EShopAgentChoices.Back:
                    AgentInput.ChangeScreen(agent, EShopScreen.Main);
                    break;
                case EShopAgentChoices.Down:
                    GetLocation.MovePosition(agent, -1);
                    break;
                case EShopAgentChoices.Up:
                    GetLocation.MovePosition(agent, 1);
                    break;
                case EShopAgentChoices.Select:
                    var state = _agentStateSelector.GetState(agent);
                    switch (state)
                    {
                        case EShopRequestStates.MakeRequest:
                            var resource = GetLocation.GetItem(agent, EShopRequestStates.MakeRequest);
                            requestSystem.MakeRequest(resource, agent.craftingInventory, agent.wallet);
                            break;
                        case EShopRequestStates.ChangePrice:

                            break;
                    }
                    break;
                case EShopAgentChoices.MakeRequest:
                    _agentStateSelector.SetState(agent, EShopRequestStates.MakeRequest);
                    break;
                case EShopAgentChoices.ChangeRequest:
                    _agentStateSelector.SetState(agent, EShopRequestStates.ChangePrice);
                    break;
                case EShopAgentChoices.IncreasePrice:
                    ChangePrice(agent, 1);
                    break;
                case EShopAgentChoices.DecreasePrice:
                    ChangePrice(agent, -1);
                    break;
            }
        }

        public void ChangePrice(ShopAgent agent, int value)
        {
            var state = _agentStateSelector.GetState(agent);
            if (state == EShopRequestStates.ChangePrice)
            {
                var resource = GetLocation.GetItem(agent, state);
                requestSystem.ChangePrice(resource, agent.craftingInventory, agent.wallet, value);
            }
        }

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
        {
            var inputChoices = new []
            {
                EShopAgentChoices.Up,
                EShopAgentChoices.Down,
                EShopAgentChoices.Select,
                EShopAgentChoices.Back,
                EShopAgentChoices.MakeRequest,
                EShopAgentChoices.ChangeRequest,
                EShopAgentChoices.IncreasePrice,
                EShopAgentChoices.DecreasePrice
            };
            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
            return outputs;
        }
    }
}
