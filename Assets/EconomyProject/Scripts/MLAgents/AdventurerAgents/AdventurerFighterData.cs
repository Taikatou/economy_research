using System;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.Inventory;
using Inventory;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerFighterData : MonoBehaviour
    {
        public int startHp = 20;
        public Sprite sprite;
        public PlayerFighterData PlayerData;
        public AdventurerInventory adventurerInventory;
        public BaseFighterData FighterData => PlayerData;

        public void Setup()
        {
            PlayerData = new PlayerFighterData
            {
                Sprite = sprite,
                UnitName = "Player",
                MaxHp = startHp,
                CurrentHp = startHp,
                onAfterAttack=OnAfterAttack,
                getUsableItem=GetUsableItem
            };
        }

        private void OnAfterAttack()
        {
            adventurerInventory.DecreaseDurability();
        }

        private UsableItem GetUsableItem()
        {
            return adventurerInventory.EquipedItem;
        }
    }
}
