using System;
using Data;
using EconomyProject.Scripts.GameEconomy.Systems.Craftsman;
using EconomyProject.Scripts.MLAgents.Shop;

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

        public override ObsData[] GetObservations(ShopAgent agent)
        {
            return Array.Empty<ObsData>() ;
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

        public override EnabledInput[] GetEnabledInputs(ShopAgent agent)
        {
            var inputChoices = new []
            {
                EShopAgentChoices.Up,
                EShopAgentChoices.Down,
                EShopAgentChoices.Select
            };
            var outputs = EconomySystemUtils<EShopAgentChoices>.GetInputOfType(inputChoices);
            return outputs;
        }
    }
}
