﻿using EconomyProject.Scripts.Inventory;


namespace TurnBased.Scripts
{
    public class PlayerFighterData : BaseFighterData
    {
        public AdventurerInventory AgentInventory;

        private UsableItem UsableItem => AgentInventory.EquipedItem;
        
        public override int Damage => UsableItem.itemDetails.damage;
        
        protected override void AfterAttack()
        {
            AgentInventory.DecreaseDurability();
        }
        
        public void ResetHp()
        {
            CurrentHp = MaxHp;
        }
    }
}
