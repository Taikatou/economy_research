using System;
using EconomyProject.Scripts.Inventory;
using TurnBased.Scripts;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents
{
    public class AdventurerFighterData : MonoBehaviour
    {
        public int startHp = 20;
        public Sprite sprite;
        public PlayerFighterData playerData;
        public AdventurerInventory adventurerInventory;
        public void Start()
        {
            playerData = new PlayerFighterData
            {
                Sprite = sprite,
                UnitName = "Player",
                MaxHp = startHp,
                CurrentHp = startHp,
                AgentInventory=adventurerInventory
            };
        }

        public void ResetHp()
        {
            playerData.CurrentHp = playerData.MaxHp;
        }

        public BaseFighterData FighterData => playerData;
    }
}
