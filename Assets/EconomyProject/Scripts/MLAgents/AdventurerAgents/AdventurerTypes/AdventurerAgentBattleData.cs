using System.Collections.Generic;
using Data;
using LevelSystem;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes
{
    public delegate void OnLevelUp(int level);
    public class AdventurerAgentBattleData : LevelUpComponent
    {
        public EAdventurerTypes adventurerType;
        
        public LevelCurve brawlerCurve;
        public LevelCurve healerCurve;
        public LevelCurve tankCurve;

        private LevelCurve AgentLevelCurve => AdventurerData[adventurerType];
        public int BonusDamage => AgentLevelCurve.levelProgressionParts[Level].damageIncrease;
        private Dictionary<EAdventurerTypes, LevelCurve> AdventurerData => new()
        {
            { EAdventurerTypes.Brawler, brawlerCurve },
            { EAdventurerTypes.Swordsman, healerCurve },
            { EAdventurerTypes.Mage, tankCurve }
        };

        private int GetLevel()
        {
            var level = 0;
            var previousExp = 0;
            foreach(var row in AgentLevelCurve.levelProgressionParts)
            {
                previousExp += row.expRequirement;
                if (TotalExp >= previousExp)
                {
                    level = row.level;
                }
            }

            return level;
        }

        private int _level;
        public override int Level => _level;
        public OnLevelUp OnLevelUp;

        protected override void LevelUpCheck()
        {
            var newLevel = GetLevel();
            if (_level != newLevel)
            {
                _level = newLevel;
                OnLevelUp?.Invoke(_level);
            }
        }
    }
}
