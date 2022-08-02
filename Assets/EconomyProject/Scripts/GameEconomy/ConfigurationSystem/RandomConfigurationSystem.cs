using System;
using System.Collections.Generic;
using Data;
using EconomyProject.Scripts.GameEconomy.DataLoggers;
using EconomyProject.Scripts.MLAgents.AdventurerAgents;
using UnityEngine;

namespace EconomyProject.Scripts.GameEconomy.ConfigurationSystem
{
    public class RandomConfigurationSystem : MonoBehaviour
    {
        public ConfigurationDataLogger configurationLogger;

        private static float SwordsmanExpBonus = 1.0f;

        private static float MageExpBonus = 1.0f;

        private static float BrawlerExpBonus = 1.0f;

        public float lowerExpRange = 0.8f;

        public float highExpRange = 1.2f;

        public void UpdateValues()
        {
            var configurationValues = new Dictionary<string, string>();
            
            var rand = new System.Random();
            var randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            SwordsmanExpBonus = (float) (lowerExpRange + randVal);
            SwordsmanExpBonus /= 5;
            configurationValues.Add("SwordsmanExpBonus", SwordsmanExpBonus.ToString());
            
            randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            MageExpBonus = (float) (lowerExpRange + randVal);
            MageExpBonus /= 5;
            configurationValues.Add("MageExpBonus", MageExpBonus.ToString());
            
            randVal = rand.NextDouble() * (highExpRange - lowerExpRange);
            BrawlerExpBonus = (float) (lowerExpRange + randVal);
            BrawlerExpBonus /= 5;
            configurationValues.Add("TankExpBonus", BrawlerExpBonus.ToString());
            
            var randomId = DateTime.Now.ToString();
            
            configurationLogger.AddData(randomId, configurationValues);
        }

        public static float GetExpBonus(EAdventurerTypes agent)
        {
            switch (agent)
            {
                case EAdventurerTypes.Brawler:
                    return BrawlerExpBonus;
                case EAdventurerTypes.Mage:
                    return MageExpBonus;
                case EAdventurerTypes.Swordsman:
                    return SwordsmanExpBonus;
                default:
                    return 0;
            }
        }
    }
}
