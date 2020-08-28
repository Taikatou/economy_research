using System;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;

namespace EconomyProject.Scripts.GameEconomy.Systems.Shop
{
    public enum AdventureShopInput {Up = AdventureShopChoices.Back + 1, Down, Select}
    public enum AdventureShopChoices { SetShop, PurchaseItem, Back }
    public class AdventurerShopSystem : StateEconomySystem<AdventureShopChoices, AdventurerAgent, EAdventurerScreen>
    {
        public AdventurerShopSubSystem adventurerShopSubSystem;

        public ShopChooserSubSystem shopChooserSubSystem;
        public override EAdventurerScreen ActionChoice => EAdventurerScreen.Shop;
        protected override AdventureShopChoices IsBackState => AdventureShopChoices.Back;
        protected override AdventureShopChoices DefaultState => AdventureShopChoices.SetShop;
        
        public override bool CanMove(AdventurerAgent agent)
        {
            return true;
        }

        public override float[] GetSenses(AdventurerAgent agent)
        {
            return new[] {0.0f};
        }

        public override InputAction[] GetInputOptions(AdventurerAgent agent)
        {
            return EconomySystemUtils.GetStateInput<AdventureShopInput>().ToArray();
        }

        protected override void MakeChoice(AdventurerAgent agent, int input)
        {
            if (Enum.IsDefined(typeof(AdventureShopInput), input))
            {
                var agentInput = (AdventureShopInput) input;
                switch (GetInputMode(agent))
                {
                    case AdventureShopChoices.SetShop:
                        shopChooserSubSystem.SetInput(agent, agentInput);
                        break;
                    case AdventureShopChoices.PurchaseItem:
                        adventurerShopSubSystem.SetInput(agent, agentInput);
                        break;
                }
            }
        }

        protected override void GoBack(AdventurerAgent agent)
        {
            AgentInput.ChangeScreen(agent, EAdventurerScreen.Main);
        }

        public void Update()
        {
            RequestDecisions();
        }
    }
}
