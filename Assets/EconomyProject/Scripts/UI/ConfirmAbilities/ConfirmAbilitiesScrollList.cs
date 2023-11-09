using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Monobehaviours;
using EconomyProject.Scripts.GameEconomy.Systems;
using EconomyProject.Scripts.GameEconomy.Systems.Adventurer;
using EconomyProject.Scripts.UI.ShopUI.ScrollLists;
using TurnBased.Scripts.AI;

namespace EconomyProject.Scripts.UI.ConfirmAbilities
{
    public struct AbilityUI
    {
        public EAttackOptions AbilityName;
    }
    public class ConfirmAbilitiesScrollList : AbstractScrollList<AbilityUI, AbilityButton>, ILastUpdate
    {
        public AdventurerSystemBehaviour adventurerSystem;

        public GetCurrentAdventurerAgent getCurrentAdventurer;

        public ConfirmAbilitiesLocationSelect confirmAbilitySelect;

        protected override List<AbilityUI> GetItemList()
        {
            var toReturn = new List<AbilityUI>();
            var agent = getCurrentAdventurer.CurrentAgent;
            if (agent != null)
            {
                foreach( var ability in PlayerActionMap.GetAbilities(agent.AdventurerType, agent.LevelComponent.Level))
                {
                    toReturn.Add(new AbilityUI{AbilityName = ability});
                }   
            }

            return toReturn;
        }

        protected override ILastUpdate LastUpdated => this;
        public void Refresh()
        {
            _lastUpdated1 = DateTime.Now;
        }

        public override void SelectItem(AbilityUI item, int number = 1)
        {
            throw new System.NotImplementedException();
        }
        
        private int _cacheIndex = -1;
        private DateTime _lastUpdated1;
        private EAdventureStates _cachedState = EAdventureStates.OutOfBattle;

        protected override void Update()
        {
            base.Update();
            var attack = confirmAbilitySelect.GetAbility(getCurrentAdventurer.CurrentAgent);
            foreach (var button in buttons)
            {
                button.UpdateButton(attack);
            }

            var state = adventurerSystem.system.GetAdventureStates(getCurrentAdventurer.CurrentAgent);
            if (getCurrentAdventurer.Index != _cacheIndex || state != _cachedState && state == EAdventureStates.ConfirmAbilities)
            {
                _cachedState = state;
                _cacheIndex = getCurrentAdventurer.Index;
                Refresh();
            }
        }

        DateTime ILastUpdate.LastUpdated => _lastUpdated1;
    }
}
