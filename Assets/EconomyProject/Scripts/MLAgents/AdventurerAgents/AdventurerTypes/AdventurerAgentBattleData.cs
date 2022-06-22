using System.Collections.Generic;
using Inventory;
using LevelSystem;
using UnityEngine;

namespace EconomyProject.Scripts.MLAgents.AdventurerAgents.AdventurerTypes
{
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
            {EAdventurerTypes.Brawler, brawlerCurve},
            {EAdventurerTypes.Healer, healerCurve},
            {EAdventurerTypes.Tank, tankCurve}
        };

        public override int Level
        {
            get
            {
                var level = 0;
                var expSum = 0;
                foreach(var row in AgentLevelCurve.levelProgressionParts)
                {
                    expSum += row.expToNextLevel;
                    if (TotalExp < expSum)
                    {
                        level = row.level;
                        break;
                    }
                    Debug.Log(TotalExp + "\t" + expSum + "\t" + row.level);
                }

                return level;
            }
        }
    }
}
