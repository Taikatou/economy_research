using System;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;
using Unity.MLAgents.Sensors;

namespace EconomyProject.Scripts.GameEconomy.Systems
{
    [Serializable]
    public class MainShopSystem : EconomySystem<ShopAgent, EShopScreen, EShopAgentChoices>
    {
        public ShopMainLocationSelect mainLocationSelect;
        public override EShopScreen ActionChoice => EShopScreen.Main;
        
        public override bool CanMove(ShopAgent agent)
        {
            return true;
        }

        public override ObsData[] GetObservations(ShopAgent agent, BufferSensorComponent[] bufferSensorComponent)
        {
            return new ObsData[]
            {
                new CategoricalObsData<EShopScreen>(mainLocationSelect.GetMenu(agent))
            };
        }

        protected override void SetChoice(ShopAgent agent, EShopAgentChoices input)
        {
            switch (input)
            {
                case EShopAgentChoices.Up:
                    mainLocationSelect.MovePosition(agent, 1);
                    break;
                case EShopAgentChoices.Down:
                    mainLocationSelect.MovePosition(agent, -1);
                    break;
                case EShopAgentChoices.Select:
                    var environment = mainLocationSelect.GetMenu(agent);
                    AgentInput.ChangeScreen(agent, environment);
                    break;
            }
        }

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent, int branch)
        {
            var screen = mainLocationSelect.GetMenu(agent);

            EShopAgentChoices[] inputChoices = null;
            if(screen == EShopScreen.Request)
            {
                inputChoices = new[]
                {
                    EShopAgentChoices.Down,
                    EShopAgentChoices.Select
                };
            }
            else
            {
                inputChoices = new[]
                {
                    EShopAgentChoices.Up,
                    EShopAgentChoices.Select
                };
            }
            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices, branch);
            return outputs;
        }
    }
}
