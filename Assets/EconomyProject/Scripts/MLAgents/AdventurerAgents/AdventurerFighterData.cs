using System;
using EconomyProject.Scripts.Interfaces;
using EconomyProject.Scripts.Inventory;
using EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes;
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

        public AdventurerAgentBattleData adventurerAgentBattleData;
        public PlayerFighterData FighterData => PlayerData;

        public void Setup()
        {
            PlayerData = new PlayerFighterData
            {
                Sprite = sprite,
                UnitName = "Player",
                MaxHp = startHp,
                CurrentHp = startHp,
                OnAfterAttack=OnAfterAttack,
                GetUsableItem=GetUsableItem,
                BonusDamage=GetLevelDamage
            };
        }

        private int GetLevelDamage()
        {
            return adventurerAgentBattleData != null ? adventurerAgentBattleData.BonusDamage : 0;
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
