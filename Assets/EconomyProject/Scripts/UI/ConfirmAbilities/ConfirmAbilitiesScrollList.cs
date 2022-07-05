using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using TurnBased.Scripts.AI;

namespace EconomyProject.Scripts.UI.ConfirmAbilities
{
    public struct AbilityUI
    {
        public string AbilityName;
    }
    public class ConfirmAbilitiesScrollList : AbstractScrollList<AbilityUI, AbilityButton>
    {
        public GetCurrentAdventurerAgent getCurrentAdventurer;
        protected override List<AbilityUI> GetItemList()
        {
            var toReturn = new List<AbilityUI>();
            var agent = getCurrentAdventurer.CurrentAgent;
            foreach( var ability in PlayerActionMap.GetAbilities(agent.AdventurerType, agent.levelComponent.Level))
            {
                var abilityName = PlayerActionMap.GetAttack(ability);
                toReturn.Add(new AbilityUI{AbilityName = abilityName.ToString()});
            }

            return toReturn;
        }
        protected override ILastUpdate LastUpdated { get; }
        public override void SelectItem(AbilityUI item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
    }
}
